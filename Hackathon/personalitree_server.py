'''

1. Unity Provides Input (Input, Personality, Description)
2. Provide the Client (Gemini) the following message:

[Your Persona]: [Description: Personas]
[Player Says]: [Input]

3. Parse output (output.text)
4. Output to Unity

@app.route('/chat', methods=['POST'])
def chat():
    data = request.json
    character = data.get('character', 'Tree_Adult')
    user_message = data.get('message', '').strip()

    # If there are no responses or message from the user when called
    if not user_message:
        random_reply = random.choice(GREETINGS.get(character, ["Hello!"]))
        return jsonify({"reply": random_reply, "status": "success"})


    system_prompt = PERSONAS.get(character, "You are a normal plant.")
    full_prompt = f"[Your Persona]: {system_prompt}\n[Player says]: {user_message}\nReply in character:"

    try:
        response = model.generate_content(full_prompt)
        return jsonify({"reply": response.text, "status": "success"})
    except Exception as e:
        return jsonify({"reply": "Connection error. Brain offline.", "status": "error"})

if __name__ == '__main__':
    app.run(port=5000)

'''

from flask import Flask, request, jsonify
from flask_cors import CORS
from google import genai
from dotenv import load_dotenv
import os

load_dotenv()
API_KEY = os.getenv('API_KEY')

client = genai.Client(api_key=API_KEY)
# chat = client.chats.create(model="gemini-2.5-flash")

app = Flask(__name__)
CORS(app)

@app.route('/chat', methods=['POST'])
def chat():

    chatInput = request.json
    description = chatInput.get('persona', 'You are a generic apple')
    input = chatInput.get('input', '')
    
    if not description:
        return jsonify({"reply": "...", "status": "success"})
    
    full_prompt = f"Response Requirement: \n20 words maximum\nNo Emojis\nNo mention of actions like you are speaking to me \n[Your Persona]: {description}\n[User Says]: {input}."
    print(full_prompt)
    try:
        response = client.models.generate_content(
            model="gemini-2.5-flash", 
            contents=full_prompt
        )
        
        return jsonify({
            "reply": response.text.strip(), 
            "status": "success"
        })
    except Exception as e:
        print(f"Error: {e}")
        return jsonify({"reply": "My stem is tingling... error!", "status": "error"})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
