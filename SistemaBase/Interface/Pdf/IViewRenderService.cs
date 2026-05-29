namespace SistemaBase.Interface.Pdf
{
    /// <summary>
    /// Allow render razor view as string
    /// </summary>
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync<T>(string viewPath, T model);
    }
}
