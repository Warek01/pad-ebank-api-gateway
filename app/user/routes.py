from http import HTTPStatus
from flask import request
from flask_restx import Namespace, Resource, Api
from google.protobuf.json_format import MessageToDict

from app.account.services import account_service
from app.generated.account_service_pb2 import GetProfileOptions, GetProfileResult
from app.generated.shared_pb2 import ServiceErrorCode


def init_user_routes(api: Api):
    user_ns = Namespace('User', path='/user')
    api.add_namespace(user_ns)

    @user_ns.route('/')
    class User(Resource):
        @user_ns.param('email')
        def get(self):
            """Get user profile"""
            options = GetProfileOptions(email=request.args['email'])
            res: GetProfileResult = account_service.getProfile(options)

            if res.HasField('error'):
                c = res.error.code
                if c == ServiceErrorCode.NOT_FOUND:
                    return MessageToDict(res.error), HTTPStatus.NOT_FOUND
                else:
                    return MessageToDict(res.error), HTTPStatus.INTERNAL_SERVER_ERROR

            return MessageToDict(res.profile)
