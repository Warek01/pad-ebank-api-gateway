using Gateway.Models;

namespace Gateway.Services;

public class TransactionServiceLoadBalancer(ServiceDiscoveryService sd)
  : LoadBalancer<TransactionServiceClient>(sd, "TransactionService");
