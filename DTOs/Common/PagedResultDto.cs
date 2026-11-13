namespace unhsjm_asistencia_backend.DTOs.Common
{
    public class PagedResultDto<T>
    {
        public int Total { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
