namespace Buhgaltery.DbClient.Model
{
    public class Settings: IEntity
    {
        public int Id { get; set; }
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
    }
}
