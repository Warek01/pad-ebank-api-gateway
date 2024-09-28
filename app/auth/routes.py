from http import HTTPStatus
from flask import request
from flask_restx import Namespace, Resource, Api, fields
from google.protobuf.json_format import MessageToDict

from app.account.services import account_service
from app.generated.account_service_pb2 import LoginCredentials, AuthResult, RegisterCredentials
from app.generated.shared_pb2 import ServiceErrorCode


def init_auth_routes(api: Api):
    auth_ns = Namespace('Auth', path='/auth')
    api.add_namespace(auth_ns)

    login_credentials_model = api.model('LoginCredentials', {
        'email':    fields.String(required=True),
        'password': fields.String(required=True),
    })

    register_credentials_model = api.model('RegisterCredentials', {
        'email':    fields.String(required=True),
        'password': fields.String(required=True),
        'fullName': fields.String(required=True),
    })

    @auth_ns.route('/login')
    @auth_ns.expect(login_credentials_model)
    class Login(Resource):
        def post(self):
            data = request.get_json()
            credentials = LoginCredentials(email=data['email'], password=data['password'])
            res: AuthResult = account_service.login(credentials)

            if res.HasField('error'):
                c = res.error.code
                if c == ServiceErrorCode.NOT_FOUND:
                    return MessageToDict(res.error), HTTPStatus.NOT_FOUND
                else:
                    MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return MessageToDict(res.credentials)

    @auth_ns.route('/register')
    @auth_ns.expect(register_credentials_model)
    class Register(Resource):
        def post(self):
            data = request.get_json()
            credentials = RegisterCredentials(email=data['email'], password=data['password'],
                                              full_name=data['fullName'])
            res: AuthResult = account_service.register(credentials)

            if res.HasField('error'):
                c = res.error.code

                if c == ServiceErrorCode.CONFLICT:
                    return MessageToDict(res.error), HTTPStatus.CONFLICT
                elif c == ServiceErrorCode.BAD_REQUEST:
                    return MessageToDict(res.error), HTTPStatus.BAD_REQUEST
                else:
                    return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return MessageToDict(res.credentials), HTTPStatus.CREATED
