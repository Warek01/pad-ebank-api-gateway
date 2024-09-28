from google.protobuf.internal import enum_type_wrapper as _enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class ServiceErrorCode(int, metaclass=_enum_type_wrapper.EnumTypeWrapper):
    __slots__ = ()
    UNKNOWN: _ClassVar[ServiceErrorCode]
    BAD_REQUEST: _ClassVar[ServiceErrorCode]
    UNAUTHORIZED: _ClassVar[ServiceErrorCode]
    NOT_FOUND: _ClassVar[ServiceErrorCode]
    CONFLICT: _ClassVar[ServiceErrorCode]
    INTERNAL: _ClassVar[ServiceErrorCode]

class Currency(int, metaclass=_enum_type_wrapper.EnumTypeWrapper):
    __slots__ = ()
    MDL: _ClassVar[Currency]
    USD: _ClassVar[Currency]
    EUR: _ClassVar[Currency]
UNKNOWN: ServiceErrorCode
BAD_REQUEST: ServiceErrorCode
UNAUTHORIZED: ServiceErrorCode
NOT_FOUND: ServiceErrorCode
CONFLICT: ServiceErrorCode
INTERNAL: ServiceErrorCode
MDL: Currency
USD: Currency
EUR: Currency

class ServiceError(_message.Message):
    __slots__ = ("code", "message")
    CODE_FIELD_NUMBER: _ClassVar[int]
    MESSAGE_FIELD_NUMBER: _ClassVar[int]
    code: ServiceErrorCode
    message: str
    def __init__(self, code: _Optional[_Union[ServiceErrorCode, str]] = ..., message: _Optional[str] = ...) -> None: ...
