import grpc

from app.generated.transaction_service_pb2 import *
from app.generated.transaction_service_pb2_grpc import TransactionServiceStub
from app.load_balancing import LoadBalancer


class TransactionService:
    load_balancer: LoadBalancer

    def __init__(self):
        self.load_balancer = LoadBalancer('TransactionService')

    def _get_channel(self) -> grpc.Channel:
        return grpc.insecure_channel(self.load_balancer.get_address())

    def _stub(self, channel: grpc.Channel) -> TransactionServiceStub:
        return TransactionServiceStub(channel)

    def transfer_currency(self, data: TransferData) -> TransferResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.TransferCurrency(data)

    def deposit_currency(self, data: DepositData) -> DepositResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.DepositCurrency(data)

    def withdraw_currency(self, data: WithdrawData) -> WithdrawResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.WithdrawCurrency(data)

    def get_history(self, options: GetHistoryOptions) -> TransactionsHistory:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.GetHistory(options)

    def cancel_transaction(self, options: CancelTransactionOptions) -> CancelTransactionResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.CancelTransaction(options)


transaction_service = TransactionService()
