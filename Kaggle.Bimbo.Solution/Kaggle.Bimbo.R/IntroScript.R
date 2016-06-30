
town.state <- read.csv("C:/Git/Kaggle.Bimbo/Data/town_state.csv")
cliente.tabla <- read.csv("C:/Git/Kaggle.Bimbo/Data/cliente_tabla.csv")
producto.tabla <- read.csv("C:/Git/Kaggle.Bimbo/Data/producto_tabla.csv")
train <- read.csv("C:/Git/Kaggle.Bimbo/Data/train.csv", header = TRUE, nrows = 20000)
test <- read.csv("C:/Git/Kaggle.Bimbo/Data/test.csv", header = TRUE)

library(caret)
modFit <- train(Demanda_uni_equil ~ Semana + Agencia_ID + Canal_ID + Ruta_SAK + Cliente_ID + Producto_ID,
                method = "gbm", data = train, verbose = FALSE)

prediction <- predict(modFit, newdata = test)
prediction[prediction < 0] = 0.01

submission <- data.frame(ID = test$id, Demanda_uni_equil = prediction)
write.csv(submission, "benchmark_submission_R", row.names = F)






