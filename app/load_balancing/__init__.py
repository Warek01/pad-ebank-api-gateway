from time import time

import grpc
import requests

from app.config.settings import *


class LoadBalancer:
    service_name: str
    addresses: list[str] | None = None
    index = 0
    last_check_time = int(time())

    def __init__(self, service_name: str):
        self.service_name = service_name

    def get_service_instances(self) -> list[dict]:
        res = requests.get(f'{SERVICE_DISCOVERY_URL}/api/service/{self.service_name}')
        res.raise_for_status()
        return res.json()['instances']

    def load_addresses(self) -> None:
        self.index = 0
        self.addresses = []
        self.last_check_time = int(time())

        instances = self.get_service_instances()

        for instance in instances:
            self.addresses.append(instance['url'])

    def get_address(self) -> str:
        if self.addresses is None or int(time()) - self.last_check_time > 30:
            self.load_addresses()

        i = self.index
        self.index += 1

        if self.index == len(self.addresses):
            self.index = 0

        return self.addresses[i]
