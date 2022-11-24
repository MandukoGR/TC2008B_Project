""" Importamos el modelo del archivo en que lo definimos. """
from initialSimulation import HighwayModel
from initialSimulation import getGrid

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

#Iniciar el modelo
model = HighwayModel(WIDTH,HEIGHT)

def updatePositions():
    global model
    global ACTUAL
    positions = []
    model.step()
    matrix = np.array(getGrid(model))
    print(matrix)
    for x in range(WIDTH):
        for y in range(HEIGHT):
            if (matrix[x, y] != 0):
                pos = [x, y, 0, matrix[x, y]]
                positions.append(pos)
                print(positions)
    return positions

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
        print(json.dumps(posDICT))
    return json.dumps(posDICT)

app = Flask(__name__, static_url_path='')

port = int(os.getenv('PORT', 8585))

@app.route('/', methods=['POST'])
def root():
    positions = updatePositions()
    resp = "Inicio exitoso del server"
    return resp

@app.route('/run', methods=['POST'])
def root():
    positions = updatePositions()
    resp = "{\"data\":" + positionsToJSON(positions) + "}"
    return resp

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=port, debug=True)

#Para ejecutar: python -m flask --app server run