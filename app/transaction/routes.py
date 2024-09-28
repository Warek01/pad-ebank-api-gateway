from http import HTTPStatus

from flask import request, current_app
from flask_restx import Namespace, Resource, Api, fields
from google.protobuf.json_format import MessageToDict

from app.account.services import account_service
from app.transaction.helpers import str_to_currency
from app.transaction.services import transaction_service
from app.generated.account_service_pb2 import *
from app.generated.transaction_service_pb2 import *
from app.generated.shared_pb2 import ServiceErrorCode


def init_transaction_routes(api: Api):
    transaction_ns = Namespace('Transaction', path='/transaction')
    api.add_namespace(transaction_ns)

    validate_transaction_model = api.model('ValidateTransaction', {
        'cardCode': fields.String(required=True),
        'currency': fields.String(required=True),
        'amount':   fields.Float(required=True),
    })

    add_currency_model = api.model('AddCurrency', {
        'currency':  fields.String(required=True),
        'amount':    fields.Integer(required=True),
        'card_code': fields.String(required=True),
    })

    change_currency_model = api.model('ChangeCurrency', {
        'currency':  fields.String(required=True),
        'card_code': fields.String(required=True),
    })

    card_identifier_model = api.model('CardIdentifier', {
        'card_code': fields.String(required=True),
    })

    transfer_model = api.model('Transfer', {
        'src_card_code': fields.String(required=True),
        'dst_card_code': fields.String(required=True),
        'currency':      fields.String(required=True),
        'amount':        fields.Float(required=True),
    })

    deposit_model = api.model('Deposit', {
        'card_code': fields.String(required=True),
        'currency':  fields.String(required=True),
        'amount':    fields.Float(required=True),
    })

    withdraw_model = api.model('Withdraw', {
        'card_code': fields.String(required=True),
        'currency':  fields.String(required=True),
        'amount':    fields.Float(required=True),
    })

    history_model = api.model('History', {
        'card_code': fields.String(required=True),
        'month':     fields.Integer(required=True),
        'year':      fields.Integer(required=True),
    })

    cancel_transaction_model = api.model('CancelTransaction', {
        'transaction_id': fields.String(required=True),
    })

    @transaction_ns.route('/validate')
    class Validate(Resource):
        @transaction_ns.expect(validate_transaction_model)
        def post(self):
            data = request.get_json()
            cur = str_to_currency(data['currency'])

            if cur is None:
                return { 'message': 'unknown currency' }, HTTPStatus.BAD_REQUEST

            current_app.logger.info(TransactionData(
                currency=cur,
                card_code=data['cardCode'],
                amount=int(data['amount'])
            ).SerializeToString())

            res = account_service.can_perform_transaction(
                TransactionData(
                    currency=cur,
                    card_code=data['cardCode'],
                    amount=int(data['amount'])
                )
            )

            if res.HasField('error'):
                c = res.error.code
                if c == ServiceErrorCode.NOT_FOUND:
                    return MessageToDict(res.error), HTTPStatus.NOT_FOUND
                else:
                    return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'result': res.can_perform }

    @transaction_ns.route('/add_currency')
    class AddCurrency(Resource):
        @transaction_ns.expect(add_currency_model)
        def post(self):
            data = request.get_json()
            cur = str_to_currency(data['currency'])

            if cur is None:
                return { 'message': 'unknown currency' }, HTTPStatus.BAD_REQUEST

            res = account_service.add_currency(AddCurrencyOptions(
                currency=cur,
                amount=data['amount'],
                card_code=data['card_code']
            ))

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'message': 'Currency added successfully' }, HTTPStatus.OK

    @transaction_ns.route('/change_currency')
    class ChangeCurrency(Resource):
        @transaction_ns.expect(change_currency_model)
        def post(self):
            data = request.get_json()
            cur = str_to_currency(data['currency'])

            if cur is None:
                return { 'message': 'unknown currency' }, HTTPStatus.BAD_REQUEST

            res = account_service.change_currency(ChangeCurrencyOptions(
                currency=cur,
                card_code=data['card_code']
            ))

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'message': 'Currency changed successfully' }, HTTPStatus.OK

    @transaction_ns.route('/block_card')
    class BlockCard(Resource):
        @transaction_ns.expect(card_identifier_model)
        def post(self):
            data = request.get_json()
            res = account_service.block_card(CardIdentifier(
                card_code=data['card_code']
            ))

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'message': 'Card blocked successfully' }, HTTPStatus.OK

    @transaction_ns.route('/unblock_card')
    class UnblockCard(Resource):
        @transaction_ns.expect(card_identifier_model)
        def post(self):
            data = request.get_json()
            res = account_service.unblock_card(CardIdentifier(
                card_code=data['card_code']
            ))

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'message': 'Card unblocked successfully' }, HTTPStatus.OK

    @transaction_ns.route('/transfer')
    class Transfer(Resource):
        @transaction_ns.expect(transfer_model)
        def post(self):
            data = request.get_json()
            cur = str_to_currency(data['currency'])

            if cur is None:
                return { 'message': 'unknown currency' }, HTTPStatus.BAD_REQUEST

            res = transaction_service.transfer_currency(
                TransferData(
                    src_card_code=data['src_card_code'],
                    dst_card_code=data['dst_card_code'],
                    currency=cur,
                    amount=int(data['amount'])
                )
            )

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'message': 'transfer successful' }, HTTPStatus.OK

    @transaction_ns.route('/deposit')
    class Deposit(Resource):
        @transaction_ns.expect(deposit_model)
        def post(self):
            data = request.get_json()
            cur = str_to_currency(data['currency'])

            if cur is None:
                return { 'message': 'unknown currency' }, HTTPStatus.BAD_REQUEST

            res = transaction_service.deposit_currency(
                DepositData(
                    card_code=data['card_code'],
                    currency=cur,
                    amount=int(data['amount'])
                )
            )

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'message': 'deposit successful' }, HTTPStatus.OK

    @transaction_ns.route('/withdraw')
    class Withdraw(Resource):
        @transaction_ns.expect(withdraw_model)
        def post(self):
            data = request.get_json()
            cur = str_to_currency(data['currency'])

            if cur is None:
                return { 'message': 'unknown currency' }, HTTPStatus.BAD_REQUEST

            res = transaction_service.withdraw_currency(
                WithdrawData(
                    card_code=data['card_code'],
                    currency=cur,
                    amount=int(data['amount'])
                )
            )

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'message': 'withdraw successful' }, HTTPStatus.OK

    @transaction_ns.route('/history')
    class GetHistory(Resource):
        @transaction_ns.expect(history_model)
        def post(self):
            data = request.get_json()
            res = transaction_service.get_history(
                GetHistoryOptions(
                    card_code=data['card_code'],
                    month=data['month'],
                    year=data['year']
                )
            )

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'transactions': MessageToDict(res.transactions) }, HTTPStatus.OK

    @transaction_ns.route('/cancel')
    class CancelTransaction(Resource):
        @transaction_ns.expect(cancel_transaction_model)
        def post(self):
            data = request.get_json()
            res = transaction_service.cancel_transaction(
                CancelTransactionOptions(
                    transaction_id=data['transaction_id']
                )
            )

            if res.HasField('error'):
                return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return { 'message': 'transaction canceled' }, HTTPStatus.OK
