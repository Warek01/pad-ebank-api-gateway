import os

from flask import Flask
from flask_restx import Api

from app.auth.routes import init_auth_routes
from app.generated.account_service_pb2 import LoginCredentials
from app.health.routes import init_health_routes
from app.transaction.routes import init_transaction_routes
from app.user.routes import init_user_routes

app: Flask
api: Api

route_initializers = [
    init_auth_routes,
    init_health_routes,
    init_user_routes,
    init_transaction_routes,
]


def create_app() -> Flask:
    global app, api

    app = Flask(
        __name__,
        instance_relative_config=True,
        static_folder='static',
        static_url_path='',
    )
    app.url_map.strict_slashes = False

    api = Api(
        app, doc='/api/docs',
        title='API Gateway documentation',
        version='1.0.0',
        prefix='/api',
    )

    for initializer in route_initializers:
        initializer(api)

    try:
        os.makedirs(app.instance_path)
    except OSError:
        pass

    return app
