using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Buhgaltery.Common
{
    /// <summary>
    /// Service of Error notify logger
    /// </summary>
    public class ErrorNotifyService : IDisposable, IErrorNotifyService
    {
        #region ConstStrings

        private const string _requiredFieldError = "ErrorNotifyService error: {0} not set"; 

        #endregion

        #region PrivateFields
        /// <summary>
        /// Flag: task collector service is connected
        /// </summary>
        private bool _isConnected = false;
        /// <summary>
        /// Flag: connect to task collector service is authed
        /// </summary>
        private bool _isAuth = false;
        /// <summary>
        /// Flag: service disposed
        /// </summary>
        private bool _isDisposed = false;
        /// <summary>
        /// Flag: can send message to task collector
        /// </summary>
        private bool _sendMessage = false;

        /// <summary>
        /// Required data for connect and send messages to task collector:
        /// - Server uri
        /// </summary>        
        private string _server;
        /// <summary>
        /// - Login
        /// </summary>
        private string _login;
        /// <summary>
        /// - Password
        /// </summary>
        private string _password;
        /// <summary>
        /// - Email for send feedback
        /// </summary>
        private string _feedback;
        /// <summary>
        /// - Title of message for default
        /// </summary>
        private string _defaultTitle;
        /// <summary>
        /// - Token for connect
        /// </summary>
        private string _token;


        /// <summary>
        /// Object for sync calls
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Calls locked
        /// </summary>
        private bool _isLock = false;

        /// <summary>
        /// Service inited
        /// </summary>
        private bool _init = false;        

        /// <summary>
        /// Logger config
        /// </summary>
        private readonly ErrorNotifyLoggerConfiguration _config;

        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="config"></param>
        public ErrorNotifyService(ErrorNotifyLoggerConfiguration config)
        {
            _config = config;
            _init = Init();
        }

        public ErrorNotifyService()
        {
           
        }

        /// <summary>
        /// Init error notify logger
        /// </summary>
        /// <returns></returns>
        private bool Init()
        {
            var options = _config.Options;
            if(options != null)
            {
                if (options.SendError)
                {
                    CheckRequired(options.Server, nameof(options.Server));
                    CheckRequired(options.Login, nameof(options.Login));
                    CheckRequired(options.Password, nameof(options.Password));

                    _sendMessage = true;
                    _server = options.Server;
                    _login = options.Login;
                    _password = options.Password;
                    _feedback = options.FeedbackContact;
                    _defaultTitle = options.DefaultTitle;

                    Task.Factory.StartNew(CheckConnect, TaskCreationOptions.LongRunning);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check required field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="name"></param>
        private void CheckRequired(string field, string name)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new Exception(string.Format(_requiredFieldError, name));
            }
        }

        private async Task<bool> Auth()
        {
            bool _isLocked = false;
            lock (_lockObject)
            {
                if (_isLock)
                {
                    _isLocked = true;
                }
                else
                {
                    _isLock = true;
                }
            }
            if (_isLocked)
            {
                for (int i = 0; i < 60; i++)
                {
                    if (!_isLock)
                    {
                        break;
                    }
                    await Task.Delay(1000);
                }
                if (!_isLock)
                {
                    if (_isAuth) return true;
                    if (_isConnected) return false;
                }
                else
                {
                    Console.WriteLine($"ErrorNotifyService: Error in Auth method: cant wait for auth with lock");
                    return false;
                }
            }

            var result = await Execute(client =>
                client.PostAsync($"{_server}/api/v1/client/auth", new ErrorNotifyClientIdentity()
                {
                    Login = _login,
                    Password = _password
                }.SerializeRequest()), "Post", s => s.ParseResponse<ErrorNotifyClientIdentityResponse>(), false);
            if (result.ResponseCode == ResponseEnum.Error)
            {
                if (_isConnected)
                {
                    Console.WriteLine($"ErrorNotifyService: Error in Auth method: wrong login or password");
                    _sendMessage = false;
                }
                return false;
            }
            _token = result.ResponseBody.Token;
            _isAuth = true;
            lock (_lockObject)
            {
                _isLock = false;
            }
            return true;
        }

        public async Task Send(string message, MessageLevelEnum level = MessageLevelEnum.Error, string title = null)
        {
            if (!_init) _init = Init();
            if (_sendMessage)
            {
                var result = await Execute(client =>
                {
                    var request = new HttpRequestMessage()
                    {
                        Headers = {
                            { HttpRequestHeader.Authorization.ToString(), $"Bearer {_token}" },
                            { HttpRequestHeader.ContentType.ToString(), "application/json" },
                        },
                        RequestUri = new Uri($"{_server}/api/v1/message/send"),
                        Method = HttpMethod.Post,
                        Content = new MessageCreator()
                        {
                            Description = message,
                            FeedbackContact = _feedback,
                            Level = (int)level,
                            Title = title ?? _defaultTitle
                        }.SerializeRequest()
                    };

                    return client.SendAsync(request);
                }, "Send", s => s.ParseResponse<MessageCreator>());

                if (result.ResponseCode == ResponseEnum.Error)
                {
                    Console.WriteLine($"ErrorNotifyService: Error in Send method: cant send message error");
                }
            }
        }

        private async Task<Response<T>> Execute<T>(
            Func<HttpClient, Task<HttpResponseMessage>> action,
            string method,
            Func<HttpResponseMessage, Task<Response<T>>> parseMethod, bool needAuth = true) where T: class
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    if (_isConnected)
                    {
                        var result = await action(client);
                        var resp =  await parseMethod(result);
                        if (resp.ResponseCode == ResponseEnum.NeedAuth)
                        {
                            if (needAuth && await Auth())
                            {
                                result = await action(client);
                                resp = await parseMethod(result);
                            }
                            else
                            {
                                return new Response<T>()
                                {
                                    ResponseCode = ResponseEnum.Error
                                };
                            }
                        }
                        return resp;
                    }
                    Console.WriteLine($"Error in {method}: server not connected");
                    return new Response<T>()
                    {
                        ResponseCode = ResponseEnum.Error
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in {method}: {ex.Message}; StackTrace: {ex.StackTrace}");
                    return new Response<T>()
                    {
                        ResponseCode = ResponseEnum.Error
                    };
                }
            }
        }

        private async Task CheckConnect()
        {
            while (!_isDisposed)
            {
                _isConnected = await CheckConnectOnce(_server);
                await Task.Delay(1000);
            }
        }

        private async Task<bool> CheckConnectOnce(string server)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var check = await client.GetAsync($"{server}/api/v1/common/ping");
                    var result = check != null && check.IsSuccessStatusCode;
                    Console.WriteLine($"Ping result: server {server} {(result ? "connected" : "disconnected")}");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in CheckConnect: {ex.Message}; StackTrace: {ex.StackTrace}");
                    return false;
                }
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}
