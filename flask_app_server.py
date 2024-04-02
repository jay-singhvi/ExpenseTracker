# Run the Flask app
# Flask --app .\flask_app_server.py run

import os
from flask import Flask, request, jsonify
from werkzeug.utils import secure_filename
from dotenv import load_dotenv
import pandas as pd

# Assuming your provided functions and setup are in a file named 'processing.py'
from processing import pdf_to_transaction_list, audio_to_transaction_list

app = Flask(__name__)
load_dotenv(dotenv_path="./../../.env", override=True)

# Configure Flask Application
UPLOAD_FOLDER = "uploads"
ALLOWED_EXTENSIONS = {"pdf", "mp3", "m4a", "wav"}

app.config["UPLOAD_FOLDER"] = UPLOAD_FOLDER
app.config["MAX_CONTENT_LENGTH"] = 16 * 1024 * 1024  # 16 MB upload limit


# Helper to check file extensions
def allowed_file(filename):
    return "." in filename and filename.rsplit(".", 1)[1].lower() in ALLOWED_EXTENSIONS


@app.route("/upload-pdf", methods=["POST"])
def upload_pdf():
    if "file" not in request.files:
        return jsonify({"error": "No file part"}), 400

    file = request.files["file"]
    if file.filename == "":
        return jsonify({"error": "No selected file"}), 400

    password = request.form.get("password")  # Get the password from the request

    if file and allowed_file(file.filename):
        filename = secure_filename(file.filename)
        file_path = os.path.join(app.config["UPLOAD_FOLDER"], filename)
        file.save(file_path)

        try:
            transactions_df = pdf_to_transaction_list(file_path, pdf_password=password)
            return jsonify(transactions_df.to_dict(orient="records")), 200
        except Exception as e:
            return jsonify({"error": str(e)}), 500


@app.route("/upload-audio", methods=["POST"])
def upload_audio():
    if "file" not in request.files:
        return jsonify({"error": "No file part"}), 400
    file = request.files["file"]
    if file.filename == "":
        return jsonify({"error": "No selected file"}), 400
    if file and allowed_file(file.filename):
        filename = secure_filename(file.filename)
        file_path = os.path.join(app.config["UPLOAD_FOLDER"], filename)
        file.save(file_path)
        try:
            transactions_df = audio_to_transaction_list(file_path)
            return jsonify(transactions_df.to_dict(orient="records")), 200
        except Exception as e:
            return jsonify({"error": str(e)}), 500


if __name__ == "__main__":
    os.makedirs(UPLOAD_FOLDER, exist_ok=True)
    app.run(debug=True)
