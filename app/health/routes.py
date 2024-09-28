from flask_restx import Resource, Api, Namespace


def init_health_routes(api: Api):
    health_ns = Namespace('Health', path='/health')
    api.add_namespace(health_ns)

    @health_ns.route('/')
    class Health(Resource):
        def get(self):
            return { 'status': 'up' }
