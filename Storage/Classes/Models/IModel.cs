namespace Storage.Classes.Models
{
    public interface IModel
    {
        /// <summary>
        /// Saves the current model in the appropriate context.
        /// </summary>
        void Save();
    }
}
