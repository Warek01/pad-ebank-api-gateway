import shared_pb2 as _shared_pb2
from google.protobuf.internal import containers as _containers
from google.protobuf.internal import enum_type_wrapper as _enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Iterable as _Iterable, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class TransactionType(int, metaclass=_enum_type_wrapper.EnumTypeWrapper):
    __slots__ = ()
    DEPOSIT: _ClassVar[TransactionType]
    WITHDRAW: _ClassVar[TransactionType]
    TRANSFER: _ClassVar[TransactionType]
DEPOSIT: TransactionType
WITHDRAW: TransactionType
TRANSFER: TransactionType

class TransferData(_message.Message):
    __slots__ = ("currency", "amount", "src_card_code", "dst_card_code")
    CURRENCY_FIELD_NUMBER: _ClassVar[int]
    AMOUNT_FIELD_NUMBER: _ClassVar[int]
    SRC_CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    DST_CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    currency: _shared_pb2.Currency
    amount: int
    src_card_code: str
    dst_card_code: str
    def __init__(self, currency: _Optional[_Union[_shared_pb2.Currency, str]] = ..., amount: _Optional[int] = ..., src_card_code: _Optional[str] = ..., dst_card_code: _Optional[str] = ...) -> None: ...

class TransferResult(_message.Message):
    __slots__ = ("error",)
    ERROR_FIELD_NUMBER: _ClassVar[int]
    error: _shared_pb2.ServiceError
    def __init__(self, error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class DepositData(_message.Message):
    __slots__ = ("currency", "amount", "card_code")
    CURRENCY_FIELD_NUMBER: _ClassVar[int]
    AMOUNT_FIELD_NUMBER: _ClassVar[int]
    CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    currency: _shared_pb2.Currency
    amount: int
    card_code: str
    def __init__(self, currency: _Optional[_Union[_shared_pb2.Currency, str]] = ..., amount: _Optional[int] = ..., card_code: _Optional[str] = ...) -> None: ...

class DepositResult(_message.Message):
    __slots__ = ("error",)
    ERROR_FIELD_NUMBER: _ClassVar[int]
    error: _shared_pb2.ServiceError
    def __init__(self, error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class WithdrawData(_message.Message):
    __slots__ = ("currency", "amount", "card_code")
    CURRENCY_FIELD_NUMBER: _ClassVar[int]
    AMOUNT_FIELD_NUMBER: _ClassVar[int]
    CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    currency: _shared_pb2.Currency
    amount: int
    card_code: str
    def __init__(self, currency: _Optional[_Union[_shared_pb2.Currency, str]] = ..., amount: _Optional[int] = ..., card_code: _Optional[str] = ...) -> None: ...

class WithdrawResult(_message.Message):
    __slots__ = ("error",)
    ERROR_FIELD_NUMBER: _ClassVar[int]
    error: _shared_pb2.ServiceError
    def __init__(self, error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class GetHistoryOptions(_message.Message):
    __slots__ = ("card_code", "month", "year")
    CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    MONTH_FIELD_NUMBER: _ClassVar[int]
    YEAR_FIELD_NUMBER: _ClassVar[int]
    card_code: str
    month: int
    year: int
    def __init__(self, card_code: _Optional[str] = ..., month: _Optional[int] = ..., year: _Optional[int] = ...) -> None: ...

class Transaction(_message.Message):
    __slots__ = ("transaction_id", "type", "src_card_code", "dst_card_code", "amount", "date")
    TRANSACTION_ID_FIELD_NUMBER: _ClassVar[int]
    TYPE_FIELD_NUMBER: _ClassVar[int]
    SRC_CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    DST_CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    AMOUNT_FIELD_NUMBER: _ClassVar[int]
    DATE_FIELD_NUMBER: _ClassVar[int]
    transaction_id: str
    type: TransactionType
    src_card_code: str
    dst_card_code: str
    amount: int
    date: str
    def __init__(self, transaction_id: _Optional[str] = ..., type: _Optional[_Union[TransactionType, str]] = ..., src_card_code: _Optional[str] = ..., dst_card_code: _Optional[str] = ..., amount: _Optional[int] = ..., date: _Optional[str] = ...) -> None: ...

class TransactionsHistory(_message.Message):
    __slots__ = ("transactions", "error")
    TRANSACTIONS_FIELD_NUMBER: _ClassVar[int]
    ERROR_FIELD_NUMBER: _ClassVar[int]
    transactions: _containers.RepeatedCompositeFieldContainer[Transaction]
    error: _shared_pb2.ServiceError
    def __init__(self, transactions: _Optional[_Iterable[_Union[Transaction, _Mapping]]] = ..., error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class CancelTransactionOptions(_message.Message):
    __slots__ = ("transaction_id",)
    TRANSACTION_ID_FIELD_NUMBER: _ClassVar[int]
    transaction_id: str
    def __init__(self, transaction_id: _Optional[str] = ...) -> None: ...

class CancelTransactionResult(_message.Message):
    __slots__ = ("error",)
    ERROR_FIELD_NUMBER: _ClassVar[int]
    error: _shared_pb2.ServiceError
    def __init__(self, error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...
