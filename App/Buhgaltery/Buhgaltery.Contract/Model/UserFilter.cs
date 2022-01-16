namespace Buhgaltery.Contract.Model
{
    /// <summary>
    /// Filter for user model
    /// </summary>
    public class UserFilter : Filter<User>
    {
        public UserFilter(int? size, int? page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
        }
        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; }
    }
}
