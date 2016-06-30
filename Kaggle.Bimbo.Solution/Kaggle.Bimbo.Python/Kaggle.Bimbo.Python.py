import numpy
import pandas
import matplotlib.pyplot as pyplot

basePath = "C:\Git\Kaggle.Bimbo\Data"
products = pandas.read_csv(basePath + "\\producto_tabla.csv")
train = pandas.read_csv(basePath + "\\train.csv")

products  =  pandas.read_csv(basePath + "\\producto_tabla.csv")
products['short_name'] = products.NombreProducto.str.extract('^(\D*)')
products['brand'] = products.NombreProducto.str.extract('^.+\s(\D+) \d+$')
weights = products.NombreProducto.str.extract('(\d+)(Kg|g)')
products['weight'] = weights[0].astype('float')*weights[1].map({'Kg':1000, 'g':1})
products['pieces'] =  products.NombreProducto.str.extract('(\d+)p ').astype('float')
products.head()
products.tail()









