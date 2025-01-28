from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import tensorflow as tf
import pickle
from tensorflow.keras.preprocessing.sequence import pad_sequences
from nltk.corpus import stopwords
from nltk.stem import WordNetLemmatizer
import nltk

nltk.download('stopwords')
nltk.download('wordnet')

# Load model and supporting files
model = tf.keras.models.load_model('model/chatbot_model.h5')

with open('model/tokenizer.pickle', 'rb') as handle:
    tokenizer = pickle.load(handle)

with open('model/label_encoder.pickle', 'rb') as handle:
    label_encoder = pickle.load(handle)

# Preprocess user input
stop_words = set(stopwords.words('english'))
lemmatizer = WordNetLemmatizer()

def preprocess_input(text):
    tokens = text.split()
    tokens = [lemmatizer.lemmatize(word.lower()) for word in tokens if word.lower() not in stop_words]
    return " ".join(tokens)

# Predict intent
def predict_intent(query):
    processed_query = preprocess_input(query)
    sequence = tokenizer.texts_to_sequences([processed_query])
    padded_sequence = pad_sequences(sequence, maxlen=model.input_shape[1], padding='post')
    prediction = model.predict(padded_sequence)
    predicted_label = label_encoder.inverse_transform([np.argmax(prediction)])[0]
    return predicted_label

# API Input/Output Models
class Query(BaseModel):
    query: str

class Response(BaseModel):
    intent: str

# Create FastAPI app
app = FastAPI()

@app.post("/predict", response_model=Response)
def predict(query: Query):
    try:
        intent = predict_intent(query.query)
        return {"intent": intent}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

# Run with Uvicorn
# Use `uvicorn main:app --reload` in the terminal
