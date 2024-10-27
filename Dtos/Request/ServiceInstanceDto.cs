using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Dtos.Request;

[SwaggerSchema("Information about a microservice instance from the service discovery")]
public class ServiceInstanceDto {
   [SwaggerSchema("Host of the microservice, like localhost")]
   [DefaultValue("localhost")]
   public string Host { get; set; } = null!;

   [SwaggerSchema("Port of the microservice, like 3000")]
   [DefaultValue("3000")]
   public string Port { get; set; } = null!;

   [SwaggerSchema("Scheme of the microservice, like https")]
   [DefaultValue("https")]
   public string Scheme { get; set; } = null!;

   public string Url() {
      return $"{Scheme}://{Host}:{Port}";
   }

   public override string ToString() {
      return Url();
   }
}
