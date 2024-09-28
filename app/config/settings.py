import os
from dotenv import load_dotenv

load_dotenv('.env')

REDIS_HOST = os.getenv('REDIS_HOST')
REDIS_PORT = int(os.getenv('REDIS_PORT'))
REDIS_DB = int(os.getenv('REDIS_DB'))
REDIS_USERNAME = os.getenv('REDIS_USER')
REDIS_PASSWORD = os.getenv('REDIS_PASSWORD')

SERVICE_DISCOVERY_URL = os.getenv('SERVICE_DISCOVERY_URL')
ACCOUNT_SERVICE_GRPC_URL = os.getenv('ACCOUNT_SERVICE_GRPC_URL')
TRANSACTION_SERVICE_GRPC_URL=os.getenv('TRANSACTION_SERVICE_GRPC_URL')
