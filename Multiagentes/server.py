""" Importamos el modelo del archivo en que lo definimos. """
from secondSimulation import HighwayModel
from secondSimulation import getGrid

""" Importamos los siguientes paquetes para el mejor manejo de valores numéricos."""
import numpy as np
import pandas as pd
import random

""" Definimos otros paquetes que vamos a usar para medir el tiempo de ejecución de nuestro algoritmo. """
import time
import datetime

""" Para coneccion con IBM """
from flask import Flask, render_template, request, jsonify
import json, logging, os, atexit

#Información de la simulación
MAX_ITER = 300
WIDTH = 3
HEIGHT = 183
POSITIONS = []

#Iniciar el modelo
model = HighwayModel(WIDTH,HEIGHT)

def updatePositions():
    global model
    model.step()
    matrix = np.array(getGrid(model))
    #print(matrix)
    for x in range(WIDTH):
        for z in range(HEIGHT):
            if (matrix[x, z] != 0):
                pos = [x, z, 0, matrix[x, z]]
                POSITIONS.append(pos)
                #print(positions)


def getPositionById(id, ps):
    # get the position with higher value in z and where value = id
    maxZ = 0
    pos = None
    for p in ps:
        if p[3] == id and p[1] > maxZ:
            maxZ = p[1]
            pos = p
    
    return pos


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
        #print(json.dumps(posDICT))
    return json.dumps(posDICT)

app = Flask(__name__, static_url_path='')

port = int(os.getenv('PORT', 8585))

@app.route('/', methods=['GET'])
def root():
    resp = "Inicio exitoso del server"
    return resp

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

@app.route('/step', methods=['GET'])
def modelStep():
    updatePositions()
    modelPosition()
    resp = "{\"data\":" + positionsToJSON(POSITIONS) + "}"
    print(model.movimientos)
    return resp




if __name__ == '__main__':
    app.run(host='127.0.0.1', port=port, debug=True)

#Para ejecutar: flask --app server.py --debug run