namespace PlantHealth.Api.Models;

public class UploadFileChunk
{
    public string Data { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public int TotalChunks { get; set; }
}