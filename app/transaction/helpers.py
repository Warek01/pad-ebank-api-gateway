from app.generated.shared_pb2 import Currency


def str_to_currency(cur: str) -> Currency | None:
    cur = cur.lower()

    if cur == 'mdl':
        return Currency.MDL
    elif cur == 'usd':
        return Currency.USD
    elif cur == 'eur':
        return Currency.EUR
    else:
        return None
