import grpc

from app.generated.account_service_pb2 import *
from app.generated.account_service_pb2_grpc import AccountServiceStub
from app.load_balancing import LoadBalancer


class AccountService:
    load_balancer: LoadBalancer

    def __init__(self):
        self.load_balancer = LoadBalancer('AccountService')

    def _get_channel(self) -> grpc.Channel:
        return grpc.insecure_channel(self.load_balancer.get_address())

    def _stub(self, channel: grpc.Channel) -> AccountServiceStub:
        return AccountServiceStub(channel)

    def login(self, credentials: LoginCredentials) -> AuthResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.Login(credentials)

    def register(self, credentials: RegisterCredentials) -> AuthResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.Register(credentials)

    def get_profile(self, options: GetProfileOptions) -> GetProfileResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.GetProfile(options)

    def add_currency(self, options: AddCurrencyOptions) -> AddCurrencyResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.AddCurrency(options)

    def can_perform_transaction(self, data: TransactionData) -> CanPerformTransactionResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.CanPerformTransaction(data)

    def change_currency(self, options: ChangeCurrencyOptions) -> ChangeCurrencyResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.ChangeCurrency(options)

    def block_card(self, id: CardIdentifier) -> BlockCardResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.BlockCard(id)

    def unblock_card(self, id: CardIdentifier) -> UnblockCardResult:
        with self._get_channel() as channel:
            stub = self._stub(channel)
            return stub.UnblockCard(id)


account_service = AccountService()
