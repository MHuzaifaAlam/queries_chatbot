import numpy as np
import tensorflow as tf
import pickle
from tensorflow.keras.preprocessing.sequence import pad_sequences
from nltk.corpus import stopwords
from nltk.stem import WordNetLemmatizer

# Load the model and supporting files
model = tf.keras.models.load_model('model/chatbot_model.h5')

with open('model/tokenizer.pickle', 'rb') as handle:
    tokenizer = pickle.load(handle)

with open('model/label_encoder.pickle', 'rb') as handle:
    le = pickle.load(handle)

# Preload intent responses
import pandas as pd
df = pd.read_csv('model/cleaned_data.csv')
intent_responses = dict(zip(df['Intents'], df['response1']))

def preprocess_input(text):
    stop_words = set(stopwords.words('english'))
    lemmatizer = WordNetLemmatizer()
    tokens = text.split()
    tokens = [lemmatizer.lemmatize(word.lower()) for word in tokens if word.lower() not in stop_words]
    return " ".join(tokens)

def predict_intent(query):
    processed_text = preprocess_input(query)
    sequence = tokenizer.texts_to_sequences([processed_text])
    padded_sequence = pad_sequences(sequence, maxlen=model.input_shape[1], padding='post')
    prediction = model.predict(padded_sequence)
    predicted_label = np.argmax(prediction)
    intent = le.inverse_transform([predicted_label])[0]
    return intent

def get_response(query):
    # Simulate the response logic
    return f"Received query: {query}"

