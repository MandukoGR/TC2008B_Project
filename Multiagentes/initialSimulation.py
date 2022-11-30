# Importamos las clases que se requieren para manejar los agentes (Agent) y su entorno (Model).
# Cada modelo puede contener múltiples agentes.
from mesa import Agent, Model 

# Debido a que necesitamos que existe un solo agente por celda, elegimos ''SingleGrid''.
from mesa.space import SingleGrid

# Con 'BaseScheduler'cada agente se activa cuando es su turno.
from mesa.time import BaseScheduler

# Haremos uso de ''DataCollector'' para obtener información de cada paso de la simulación.
from mesa.datacollection import DataCollector

# Importamos los siguientes paquetes para el mejor manejo de valores numéricos.
import numpy as np
import pandas as pd

# Definimos otros paquetes que vamos a usar para medir el tiempo de ejecución de nuestro algoritmo.
import time
import datetime


##--------------------------------------------------------------------------------------------------

"""
@brief: Función que genera grid
@param: model: modelo (HighwayModel)
@return: grid: grid
"""

def getGrid(model):
    grid = np.zeros( (model.grid.width, model.grid.height) )
    for (content, x, y) in model.grid.coord_iter():
        if model.grid.is_cell_empty((x,y)):
            grid[x][y] = 0
        else:
            grid[x][y] = content.unique_id

    return grid


##--------------------------------------------------------------------------------------------------

"""
@brief: Clase que representa a un agente.
@param: unique_id: identificador único del agente.
@param: model: modelo (HighwayModel).
@param: initialLane: carril inicial.
@param: fullyFunctional: booleano que indica si el agente es el que debe parar.

@atribute speed: velocidad del agente.
@atribute lane: carril del agente.
@atribute functional: booleano que indica si el agente es el que debe parar.
@atribute changedLane: booleano que indica si el agente cambió de carril.

@method: getFrontEmptySpaces: obtiene los espacios vacíos delanteros.
@method: getFrontVehicle: obtiene el vehículo delantero ( hasta 3 espacios).
@method: whereToChange: Regresa coordenadas de la celda a la que se moverá el agente.
@method: changeLane: Mueve el agente a la celda indicada (lo cambia de carril).
@method: returnToLane: Mueve el agente a la celda indicada (lo regresa a su carril si anteriromente cambió).
@method: step: Función que se ejecuta en cada paso de la simulación.



"""
class VehicleAgent(Agent):

    def __init__(self, unique_id, model, initialLane,fullyFunctional):
        super().__init__(unique_id, model)
        self.speed = 3
        self.lane = initialLane
        self.functional = fullyFunctional
        self.changedLane = False
    def getFrontEmptySpaces(self):
        frontEmptySpaces = 0
        for i in range(1,4):
            if self.pos[1]+i <= self.model.grid.height-1 and self.model.grid.is_cell_empty((self.pos[0],self.pos[1]+i)):
                frontEmptySpaces += 1
            else:
                break
        return frontEmptySpaces
    def getFrontVehicle(self):
        frontVehicle = None
        for i in range(1,4):
            if self.pos[1]+i <= self.model.grid.height-1 and not self.model.grid.is_cell_empty((self.pos[0],self.pos[1]+i)):
                frontVehicle = self.model.grid.get_cell_list_contents([(self.pos[0],self.pos[1]+i)])[0]
                break
        return frontVehicle
    def whereToChange(self):
        rightLane = ()
        leftlane = ()
        if self.model.grid.is_cell_empty((self.pos[0]+1, self.pos[1])):
            rightLane = (self.pos[0]+1, self.pos[1])
        elif self.model.grid.is_cell_empty((self.pos[0]-1, self.pos[1])):
            leftlane = (self.pos[0]-1, self.pos[1])
       
        if leftlane and rightLane:
            lane = np.random.choice([leftlane,rightLane])
            return lane
        elif rightLane:
            return rightLane
        elif leftlane:
            return leftlane
        else:
            return self.pos
    def changeLane(self,laneToChange):
        self.model.grid.move_agent(self,laneToChange)
        self.lane = laneToChange[0]
        self.step()
    
    def returnToLane(self):
        self.model.grid.move_agent(self,(1,self.pos[1]))
        self.changedLane = False
        self.step()

    def step(self):
        if  self.functional==0 and self.pos[1] == 90:
            self.speed = 0
        frontEmptySpaces = self.getFrontEmptySpaces()
        frontVehicle = self.getFrontVehicle()
        if self.changedLane== True and self.pos[1] > 90:
            self.returnToLane()
        if frontVehicle is not None:
            if frontEmptySpaces == 2 or frontEmptySpaces == 1:
                if self.lane == 1 and frontVehicle.speed != self.speed:
                    laneToChange = self.whereToChange()
                    if laneToChange != self.pos:
                        self.changeLane(laneToChange)
                        self.changedLane = True
                    else:
                        self.speed = 1
                elif self.lane == 0 or self.lane == 2:
                    self.speed = frontVehicle.speed
            if frontEmptySpaces == 0:
                if self.lane == 1 and frontVehicle.speed != self.speed:
                    laneToChange = self.whereToChange()
                    if laneToChange != self.pos:
                        self.changeLane(laneToChange)
                        self.changedLane = True
                    else:
                        self.speed = 0
                elif self.lane == 2 or self.lane == 3:
                    self.speed = frontVehicle.speed
                if self.pos[1]+self.speed >= self.model.grid.height-1:
                    self.model.grid.remove_agent(self)
                    self.model.schedule.remove(self)
                else:
                    if self.functional == 1:
                        self.model.grid.move_agent(self,(self.pos[0], self.pos[1]+self.speed))
                    else:
                        if self.pos[1] < 90:
                            self.model.grid.move_agent(self,(self.pos[0], self.pos[1]+self.speed))
                        else:
                            self.speed = 0
        else:
            if self.pos[1]+self.speed >= self.model.grid.height-1:
                self.model.grid.remove_agent(self)
                self.model.schedule.remove(self)
            else:
                self.model.grid.move_agent(self,(self.pos[0],self.pos[1]+self.speed))


##--------------------------------------------------------------------------------------------------
"""
@brief: Clase que representa al modelo.
@param width: ancho del grid.
@param height: alto del grid.

@atribute movimientos : contador de steps.
@atribute grid: grid del modelo.
@atribute schedule: schedule del modelo (BaseScheduler).
@atribute datacollector: datacollector del modelo (DataCollector).

@method: step: Función que se ejecuta en cada paso de la simulación.

@note:  El agente que se para es el 120, tarda 30 movimientos en llegar a la mitad del grid
de esta forma este se para en el segundo 150 (la mitad de la simulación)

"""

class HighwayModel(Model):

    def __init__(self,width,height):
        self.movimientos = 0
        self.numAgentsCreated = 0
        self.grid = SingleGrid(width,height,False)
        self.schedule = BaseScheduler(self)
        self.datacollector = DataCollector(model_reporters={"Grid":getGrid})
   
    def step(self):
        initialRail = np.random.choice([0,1,2])
        if self.numAgentsCreated != 120:
            car = VehicleAgent(self.numAgentsCreated,self,initialRail,1)
            self.grid.place_agent( car,(initialRail,0) )
            self.schedule.add(car)
            self.numAgentsCreated += 1 
        else: 
            car = VehicleAgent(self.numAgentsCreated,self,1,0)
            self.grid.place_agent( car,(1,0) )
            self.schedule.add(car)
            self.numAgentsCreated += 1 
        self.datacollector.collect(self)
        self.schedule.step()
        self.movimientos += 1

##--------------------------------------------------------------------------------------------------