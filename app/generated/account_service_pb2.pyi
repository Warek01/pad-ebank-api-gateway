import shared_pb2 as _shared_pb2
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class LoginCredentials(_message.Message):
    __slots__ = ("email", "password")
    EMAIL_FIELD_NUMBER: _ClassVar[int]
    PASSWORD_FIELD_NUMBER: _ClassVar[int]
    email: str
    password: str
    def __init__(self, email: _Optional[str] = ..., password: _Optional[str] = ...) -> None: ...

class AuthCredentials(_message.Message):
    __slots__ = ("email", "full_name")
    EMAIL_FIELD_NUMBER: _ClassVar[int]
    FULL_NAME_FIELD_NUMBER: _ClassVar[int]
    email: str
    full_name: str
    def __init__(self, email: _Optional[str] = ..., full_name: _Optional[str] = ...) -> None: ...

class AuthResult(_message.Message):
    __slots__ = ("credentials", "error")
    CREDENTIALS_FIELD_NUMBER: _ClassVar[int]
    ERROR_FIELD_NUMBER: _ClassVar[int]
    credentials: AuthCredentials
    error: _shared_pb2.ServiceError
    def __init__(self, credentials: _Optional[_Union[AuthCredentials, _Mapping]] = ..., error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class RegisterCredentials(_message.Message):
    __slots__ = ("email", "password", "full_name")
    EMAIL_FIELD_NUMBER: _ClassVar[int]
    PASSWORD_FIELD_NUMBER: _ClassVar[int]
    FULL_NAME_FIELD_NUMBER: _ClassVar[int]
    email: str
    password: str
    full_name: str
    def __init__(self, email: _Optional[str] = ..., password: _Optional[str] = ..., full_name: _Optional[str] = ...) -> None: ...

class GetProfileOptions(_message.Message):
    __slots__ = ("email",)
    EMAIL_FIELD_NUMBER: _ClassVar[int]
    email: str
    def __init__(self, email: _Optional[str] = ...) -> None: ...

class Profile(_message.Message):
    __slots__ = ("email", "full_name")
    EMAIL_FIELD_NUMBER: _ClassVar[int]
    FULL_NAME_FIELD_NUMBER: _ClassVar[int]
    email: str
    full_name: str
    def __init__(self, email: _Optional[str] = ..., full_name: _Optional[str] = ...) -> None: ...

class GetProfileResult(_message.Message):
    __slots__ = ("profile", "error")
    PROFILE_FIELD_NUMBER: _ClassVar[int]
    ERROR_FIELD_NUMBER: _ClassVar[int]
    profile: Profile
    error: _shared_pb2.ServiceError
    def __init__(self, profile: _Optional[_Union[Profile, _Mapping]] = ..., error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class AddCurrencyOptions(_message.Message):
    __slots__ = ("currency", "amount", "card_code")
    CURRENCY_FIELD_NUMBER: _ClassVar[int]
    AMOUNT_FIELD_NUMBER: _ClassVar[int]
    CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    currency: _shared_pb2.Currency
    amount: int
    card_code: str
    def __init__(self, currency: _Optional[_Union[_shared_pb2.Currency, str]] = ..., amount: _Optional[int] = ..., card_code: _Optional[str] = ...) -> None: ...

class AddCurrencyResult(_message.Message):
    __slots__ = ("error",)
    ERROR_FIELD_NUMBER: _ClassVar[int]
    error: _shared_pb2.ServiceError
    def __init__(self, error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class TransactionData(_message.Message):
    __slots__ = ("currency", "amount", "card_code")
    CURRENCY_FIELD_NUMBER: _ClassVar[int]
    AMOUNT_FIELD_NUMBER: _ClassVar[int]
    CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    currency: _shared_pb2.Currency
    amount: int
    card_code: str
    def __init__(self, currency: _Optional[_Union[_shared_pb2.Currency, str]] = ..., amount: _Optional[int] = ..., card_code: _Optional[str] = ...) -> None: ...

class CanPerformTransactionResult(_message.Message):
    __slots__ = ("can_perform", "error")
    CAN_PERFORM_FIELD_NUMBER: _ClassVar[int]
    ERROR_FIELD_NUMBER: _ClassVar[int]
    can_perform: bool
    error: _shared_pb2.ServiceError
    def __init__(self, can_perform: bool = ..., error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class ChangeCurrencyOptions(_message.Message):
    __slots__ = ("currency", "card_code")
    CURRENCY_FIELD_NUMBER: _ClassVar[int]
    CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    currency: _shared_pb2.Currency
    card_code: str
    def __init__(self, currency: _Optional[_Union[_shared_pb2.Currency, str]] = ..., card_code: _Optional[str] = ...) -> None: ...

class ChangeCurrencyResult(_message.Message):
    __slots__ = ("error",)
    ERROR_FIELD_NUMBER: _ClassVar[int]
    error: _shared_pb2.ServiceError
    def __init__(self, error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class CardIdentifier(_message.Message):
    __slots__ = ("card_code",)
    CARD_CODE_FIELD_NUMBER: _ClassVar[int]
    card_code: str
    def __init__(self, card_code: _Optional[str] = ...) -> None: ...

class BlockCardResult(_message.Message):
    __slots__ = ("error",)
    ERROR_FIELD_NUMBER: _ClassVar[int]
    error: _shared_pb2.ServiceError
    def __init__(self, error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...

class UnblockCardResult(_message.Message):
    __slots__ = ("error",)
    ERROR_FIELD_NUMBER: _ClassVar[int]
    error: _shared_pb2.ServiceError
    def __init__(self, error: _Optional[_Union[_shared_pb2.ServiceError, _Mapping]] = ...) -> None: ...
