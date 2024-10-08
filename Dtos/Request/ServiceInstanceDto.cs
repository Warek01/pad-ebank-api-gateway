namespace Gateway.Dtos.Request;

public class ServiceInstanceDto {
  public string Host { get; set; } = null!;
  public string Port { get; set; } = null!;
  public string Scheme { get; set; } = null!;

  public string Url() {
    return $"{Scheme}://{Host}:{Port}";
  }

  public override string ToString() {
    return Url();
  }
}
