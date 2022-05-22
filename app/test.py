import math
import time
from flask import Flask


class SHomeHelper:
    @staticmethod
    def format_log_address(mac: bytes, port: int):
        return f"{SHomeHelper.format_mac(mac)}/{port}"

    @staticmethod
    def format_mac(mac: bytes):
        return ":".join(["{:02X}".format(b) for b in bytes(mac)])

    @staticmethod
    def mac_to_bytes(mac: str):
        return bytes([int(c, 16) for c in mac.split(":")])

    @staticmethod
    def log_address_to_bytes(mac: str):
        macParsed = mac.split("/")
        if len(macParsed) != 2:
            raise Exception("Invalid mac")

        return int(macParsed[0]).to_bytes(2, 'little') + SHomeHelper.mac_to_bytes(macParsed[1])

    @staticmethod
    def format_api_result(data):
        return {"result": data}


app = Flask(__name__, )

updates = {}
state = {}

@app.route('/api/<method_name>/')
def hello_world(method_name, req_id=0):
    if method_name == "list-devices":
        return {"data": [
            {
                "address":    "FF:FF:FF:FF:FF:FF/22",
                "attribs":    {},
                "dev_class":    2,
                "group_name":    "",
                "name":    "Lamp",
            },
            {
                "address":    "FF:FF:FF:FF:AA:BB/7",
                "attribs":    {},
                "dev_class":    3,
                "group_name":    "",
                "name":    "Room",
            },
            {
                "address": "FF:FF:FF:FF:FF:FF/27",
                "attribs": {},
                "dev_class": 2,
                "group_name": "",
                "name": "LL",
            }
        ]}
    return {}


@app.route('/api/device/<mac>/<int:port>/<device_type>/<method_name>')
def device(mac, port, device_type, method_name, req_id=0):
    if method_name == "state":
        if device_type == "relay":
            return {"data": {"status": state.get(f"{mac}/{port}", "0")}}
        if device_type == "temp-sensor":
            return {"data": {"humidity": 67, "temperature": 26}}
    if method_name == "1" or method_name == "0":
        state[f"{mac}/{port}"] = method_name;
        updates[f"{mac}/{port}"] = {"data": {"status": method_name}}
    return {"data": {"status": "ok"}}


@app.route('/api/webhook/new')
def webhook_new():
    return {"data": {"status": "ok", "uuid": "test"}}


@app.route('/api/webhook/<uuid>')
def webhook_get(uuid):
    upd = updates.copy()
    updates.clear()

    updates["FF:FF:FF:FF:AA:BB/7"] = {"data": {"humidity": 67 + math.sin(time.time()) * 10, "temperature": 26 + math.cos(time.time()) * 10}}

    return {"data": upd}


@app.route('/api/webhook/<uuid>/add/<addr>/<int:port>')
def webhook_add(uuid, addr, port):
    return {"data": {"status": "ok", "id": 0}}

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8080)
