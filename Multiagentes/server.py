""" Importamos el modelo del archivo en que lo definimos. """
from initialSimulation import HighwayModel
from initialSimulation import getGrid

""" Importamos los siguientes paquetes para el mejor manejo de valores numéricos."""
import numpy as np

""" Para coneccion con IBM """
from flask import Flask, request
import json, os

"""
@param: model: WIDTH (HighwayModel)
@param: model: HEIGHT (HighwayModel)
@param: POSITIONS: Arreglo que contiene todas las posiciones de los agentes de todas las iteraciones
"""
WIDTH = 3
HEIGHT = 183
POSITIONS = []

"""
@brief: Instancia de la clase que representa al modelo.
@param width: ancho del grid.
@param height: alto del grid.

@method: getGrid: Función que reporta la situación del grid en cada paso de la simulación.
@method: step: Función que se ejecuta en cada paso de la simulación.
"""
model = HighwayModel(WIDTH,HEIGHT)

"""
@brief: Función que actualiza y añade todas las posiciones recolectadas por la funcion getGrid
@method: getGrid: Función que reporta la situación del grid en cada paso de la simulación.
"""
def updatePositions():
    global model
    model.step()
    matrix = np.array(getGrid(model))
    for x in range(WIDTH):
        for z in range(HEIGHT):
            if (matrix[x, z] != 0):
                pos = [x, z, 0, matrix[x, z]]
                POSITIONS.append(pos)

"""
@brief: Función que regresa las ultimas coordenadas de un agente en base a su id.
@param: id: id del vehiculo del que se desea conocer sus ultimas coordenadas.
@param: ps: Lista que contiene todas las posiciones que se han recibido por parte de la función getGrid.
@return: pos: [x,y,z,value] del agente con el id igual a value.
"""
def getPositionById(id, ps):
    maxZ = 0
    pos = None
    for p in ps:
        if p[3] == id and p[1] > maxZ:
            maxZ = p[1]
            pos = p
    return pos

"""
@brief: Función que obtiene la velocidad promedio de todos los agentes activos.
@return: meanSpeed: velocidad promedio de todos los agentes activos.
"""
def getMeanSpeed():
    global model
    meanSpeed = 0
    for agent in model.schedule.agents:
        meanSpeed += agent.speed
    meanSpeed = meanSpeed / len(model.schedule.agents)
    return meanSpeed

"""
@brief: Función que convierte las posiciones recibidas del grid a un JSON
@param: ps: Lista que contiene todas las posiciones que se han recibido por parte de la función getGrid.
@return: json.dumps(posDICT): JSON que contiene todas las coordenadas recibidas del grid.
"""
def positionsToJSON(ps):
    posDICT = []
    for p in ps:
        pos = {
            "x" : p[0],
            "z" : p[1],
            "y" : p[2],
            "val" : p[3]
        }
        posDICT.append(pos)
    return json.dumps(posDICT)

"""
@param: APP: Configuración inicial del servidor de Flask.
@param: port: Puerto del equipo que se desea usar para ejecutar el servidor de Flask dentro del localhost.
"""
app = Flask(__name__, static_url_path='')
port = int(os.getenv('PORT', 8585))

"""
@brief: Función por default del server
@return: resp: String que confirma conexión exitosa con el server.
"""
@app.route('/', methods=['GET'])
def root():
    resp = "Inicio exitoso del server"
    return resp

"""
@brief: Función que regresa las coordendas más recientes de un vehiculo por medio de su id.
@param: id: Id del vehiculo.
@return: pos: JSON que contiene las coordenadas más recientes del agente.
@return: resp: String que confirma que el agente no esta activo o se recibio mal el id del agente.
"""
@app.route('/position', methods=['GET'])
def modelPosition():
    args = request.args
    id = args.get('id')
    if id is not None:
        id = float(id)
        pos = getPositionById(id, POSITIONS)
        if pos is not None:
            pos = positionsToJSON([pos])
            return pos
        else:
            resp = "Agente llego a final"
        return resp
    else:
        resp = "No se ingreso id"
        return resp

"""
@brief: Función que de un paso en el modelo y actualiza las posiciones de los agentes.
@return: resp: JSON que contiene las coordenadas de todos los agentes hasta el momento dado de la simulación.
"""
@app.route('/step', methods=['GET'])
def modelStep():
    updatePositions()
    modelPosition()
    resp = "{\"data\":" + positionsToJSON(POSITIONS) + "}"
    print(model.movimientos)
    return resp

"""
@brief: Función que obtiene la velocidad promedio de todos los vehiculos activos en la simulación.
@return: resp: JSON que contiene velocidad promedio de todos los agentes.
"""
@app.route('/speed', methods=['GET'])
def modelSpeed():
    speed = getMeanSpeed()
    resp = "{\"data\":" + str(speed) + "}"
    return resp

"""
@brief: Función main que contiene el host (designado a localhost), ajusta el puerto a lo configurado anteiormente y activa la propiedad degub de Flask.
"""
if __name__ == '__main__':
    app.run(host='127.0.0.1', port=port, debug=True)

#Para ejecutar: flask --app server.py --debug run