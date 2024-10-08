using Gateway.Models;

namespace Gateway.Services;

public class AccountServiceLoadBalancer(ServiceDiscoveryService sd)
  : LoadBalancer<AccountServiceClient>(sd, "AccountService");
