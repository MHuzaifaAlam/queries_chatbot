from fastapi import FastAPI
from pydantic import BaseModel
import tensorflow as tf
import numpy as np
import pickle

# Initialize FastAPI app
app = FastAPI()

# Load the trained model
model = tf.keras.models.load_model("chatbot_model.h5")

# Load the tokenizer
with open("tokenizer.pickle", "rb") as handle:
    tokenizer = pickle.load(handle)

# Load the label encoder
with open("label_encoder.pickle", "rb") as handle:
    label_encoder = pickle.load(handle)

# Define the input schema
class QueryInput(BaseModel):
    query: str

# Preprocessing function
def preprocess_input(query, tokenizer, max_len):
    # Tokenize and pad the input
    tokens = tokenizer.texts_to_sequences([query])
    padded_tokens = tf.keras.preprocessing.sequence.pad_sequences(tokens, maxlen=max_len, padding="post")
    return padded_tokens

# Define the prediction endpoint
@app.post("/predict")
def predict_intent(input_data: QueryInput):
    # Preprocess the user input
    query = input_data.query
    max_len = model.input_shape[1]  # Get max_len from the model's input shape
    processed_query = preprocess_input(query, tokenizer, max_len)

    # Make a prediction
    predictions = model.predict(processed_query)
    predicted_label = np.argmax(predictions, axis=1)  # Get the index of the highest probability

    # Decode the label to get the intent
    intent = label_encoder.inverse_transform(predicted_label)[0]

    return {"intent": intent}
