# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# NO CHECKED-IN PROTOBUF GENCODE
# source: account_service.proto
# Protobuf Python Version: 5.27.2
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import runtime_version as _runtime_version
from google.protobuf import symbol_database as _symbol_database
from google.protobuf.internal import builder as _builder
_runtime_version.ValidateProtobufRuntimeVersion(
    _runtime_version.Domain.PUBLIC,
    5,
    27,
    2,
    '',
    'account_service.proto'
)
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()


import app.generated.shared_pb2 as shared__pb2


DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x15\x61\x63\x63ount_service.proto\x12\x0f\x61\x63\x63ount_service\x1a\x0cshared.proto\"3\n\x10LoginCredentials\x12\r\n\x05\x65mail\x18\x01 \x01(\t\x12\x10\n\x08password\x18\x02 \x01(\t\"3\n\x0f\x41uthCredentials\x12\r\n\x05\x65mail\x18\x01 \x01(\t\x12\x11\n\tfull_name\x18\x02 \x01(\t\"v\n\nAuthResult\x12\x37\n\x0b\x63redentials\x18\x01 \x01(\x0b\x32 .account_service.AuthCredentialsH\x00\x12%\n\x05\x65rror\x18\x02 \x01(\x0b\x32\x14.shared.ServiceErrorH\x00\x42\x08\n\x06result\"I\n\x13RegisterCredentials\x12\r\n\x05\x65mail\x18\x01 \x01(\t\x12\x10\n\x08password\x18\x02 \x01(\t\x12\x11\n\tfull_name\x18\x03 \x01(\t\"\"\n\x11GetProfileOptions\x12\r\n\x05\x65mail\x18\x01 \x01(\t\"+\n\x07Profile\x12\r\n\x05\x65mail\x18\x01 \x01(\t\x12\x11\n\tfull_name\x18\x02 \x01(\t\"p\n\x10GetProfileResult\x12+\n\x07profile\x18\x01 \x01(\x0b\x32\x18.account_service.ProfileH\x00\x12%\n\x05\x65rror\x18\x02 \x01(\x0b\x32\x14.shared.ServiceErrorH\x00\x42\x08\n\x06result\"[\n\x12\x41\x64\x64\x43urrencyOptions\x12\"\n\x08\x63urrency\x18\x01 \x01(\x0e\x32\x10.shared.Currency\x12\x0e\n\x06\x61mount\x18\x02 \x01(\x05\x12\x11\n\tcard_code\x18\x03 \x01(\t\"8\n\x11\x41\x64\x64\x43urrencyResult\x12#\n\x05\x65rror\x18\x01 \x01(\x0b\x32\x14.shared.ServiceError\"X\n\x0fTransactionData\x12\"\n\x08\x63urrency\x18\x01 \x01(\x0e\x32\x10.shared.Currency\x12\x0e\n\x06\x61mount\x18\x02 \x01(\x05\x12\x11\n\tcard_code\x18\x03 \x01(\t\"e\n\x1b\x43\x61nPerformTransactionResult\x12\x15\n\x0b\x63\x61n_perform\x18\x01 \x01(\x08H\x00\x12%\n\x05\x65rror\x18\x02 \x01(\x0b\x32\x14.shared.ServiceErrorH\x00\x42\x08\n\x06result\"N\n\x15\x43hangeCurrencyOptions\x12\"\n\x08\x63urrency\x18\x01 \x01(\x0e\x32\x10.shared.Currency\x12\x11\n\tcard_code\x18\x02 \x01(\t\";\n\x14\x43hangeCurrencyResult\x12#\n\x05\x65rror\x18\x01 \x01(\x0b\x32\x14.shared.ServiceError\"#\n\x0e\x43\x61rdIdentifier\x12\x11\n\tcard_code\x18\x01 \x01(\t\"6\n\x0f\x42lockCardResult\x12#\n\x05\x65rror\x18\x01 \x01(\x0b\x32\x14.shared.ServiceError\"8\n\x11UnblockCardResult\x12#\n\x05\x65rror\x18\x01 \x01(\x0b\x32\x14.shared.ServiceError2\xc3\x05\n\x0e\x41\x63\x63ountService\x12G\n\x05Login\x12!.account_service.LoginCredentials\x1a\x1b.account_service.AuthResult\x12M\n\x08Register\x12$.account_service.RegisterCredentials\x1a\x1b.account_service.AuthResult\x12S\n\nGetProfile\x12\".account_service.GetProfileOptions\x1a!.account_service.GetProfileResult\x12V\n\x0b\x41\x64\x64\x43urrency\x12#.account_service.AddCurrencyOptions\x1a\".account_service.AddCurrencyResult\x12g\n\x15\x43\x61nPerformTransaction\x12 .account_service.TransactionData\x1a,.account_service.CanPerformTransactionResult\x12_\n\x0e\x43hangeCurrency\x12&.account_service.ChangeCurrencyOptions\x1a%.account_service.ChangeCurrencyResult\x12N\n\tBlockCard\x12\x1f.account_service.CardIdentifier\x1a .account_service.BlockCardResult\x12R\n\x0bUnblockCard\x12\x1f.account_service.CardIdentifier\x1a\".account_service.UnblockCardResultb\x06proto3')

_globals = globals()
_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, _globals)
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'account_service_pb2', _globals)
if not _descriptor._USE_C_DESCRIPTORS:
  DESCRIPTOR._loaded_options = None
  _globals['_LOGINCREDENTIALS']._serialized_start=56
  _globals['_LOGINCREDENTIALS']._serialized_end=107
  _globals['_AUTHCREDENTIALS']._serialized_start=109
  _globals['_AUTHCREDENTIALS']._serialized_end=160
  _globals['_AUTHRESULT']._serialized_start=162
  _globals['_AUTHRESULT']._serialized_end=280
  _globals['_REGISTERCREDENTIALS']._serialized_start=282
  _globals['_REGISTERCREDENTIALS']._serialized_end=355
  _globals['_GETPROFILEOPTIONS']._serialized_start=357
  _globals['_GETPROFILEOPTIONS']._serialized_end=391
  _globals['_PROFILE']._serialized_start=393
  _globals['_PROFILE']._serialized_end=436
  _globals['_GETPROFILERESULT']._serialized_start=438
  _globals['_GETPROFILERESULT']._serialized_end=550
  _globals['_ADDCURRENCYOPTIONS']._serialized_start=552
  _globals['_ADDCURRENCYOPTIONS']._serialized_end=643
  _globals['_ADDCURRENCYRESULT']._serialized_start=645
  _globals['_ADDCURRENCYRESULT']._serialized_end=701
  _globals['_TRANSACTIONDATA']._serialized_start=703
  _globals['_TRANSACTIONDATA']._serialized_end=791
  _globals['_CANPERFORMTRANSACTIONRESULT']._serialized_start=793
  _globals['_CANPERFORMTRANSACTIONRESULT']._serialized_end=894
  _globals['_CHANGECURRENCYOPTIONS']._serialized_start=896
  _globals['_CHANGECURRENCYOPTIONS']._serialized_end=974
  _globals['_CHANGECURRENCYRESULT']._serialized_start=976
  _globals['_CHANGECURRENCYRESULT']._serialized_end=1035
  _globals['_CARDIDENTIFIER']._serialized_start=1037
  _globals['_CARDIDENTIFIER']._serialized_end=1072
  _globals['_BLOCKCARDRESULT']._serialized_start=1074
  _globals['_BLOCKCARDRESULT']._serialized_end=1128
  _globals['_UNBLOCKCARDRESULT']._serialized_start=1130
  _globals['_UNBLOCKCARDRESULT']._serialized_end=1186
  _globals['_ACCOUNTSERVICE']._serialized_start=1189
  _globals['_ACCOUNTSERVICE']._serialized_end=1896
# @@protoc_insertion_point(module_scope)
