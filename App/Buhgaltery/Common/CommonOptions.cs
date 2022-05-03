//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System.Collections.Generic;

namespace Buhgaltery.Common
{
    /// <summary>
    /// Класс хранения настроек
    /// </summary>
    public class CommonOptions
    {
        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public Dictionary<string, string> ConnectionStrings { get; set; }
        /// <summary>
        /// Настройки отправки сообщений об ошибках
        /// </summary>
        public ErrorNotifyOptions ErrorNotifyOptions { get; set; }       
        /// <summary>
        /// Настройки авторизации
        /// </summary>
        public AuthOptions AuthOptions { get; set; }
    }
}