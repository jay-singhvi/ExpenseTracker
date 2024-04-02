import os
from io import StringIO
import tabula

import pandas as pd
from dotenv import load_dotenv
from openai import OpenAI

load_dotenv(dotenv_path="./../../.env", override=True)
client = OpenAI(api_key=os.environ.get("OPENAI_API_KEY"))
# Specify the project root directory
root_dir = os.getcwd()
# Define the path to the ffmpeg bin directory
ffmpeg_bin_path = root_dir + r"\ffmpeg-n6.1-latest-win64-gpl-6.1\bin"
if ffmpeg_bin_path not in os.environ["PATH"]:
    os.environ["PATH"] = ffmpeg_bin_path + ";" + os.environ["PATH"]

# Error: Skipping analyzing "pydub": module is installed, but missing library stubs or py.typed marker  [import-untyped]
from pydub import AudioSegment


def model_response_to_dataframe(model_response: str) -> pd.DataFrame:
    # Check if the first line contains column names
    has_column_names = any(char.isalpha() for char in model_response.split("\n")[0])
    # Create a StringIO object to simulate a file-like object
    transaction_list_string = StringIO(model_response)
    # Read CSV into a DataFrame, adjusting the header parameter based on whether column names are present or not
    transaction_list_dataframe = pd.read_csv(
        transaction_list_string,
        sep=",",
        header=(
            0 if has_column_names else None
        ),  # Set header to 0 if column names are present, otherwise None
        names=(
            ["date", "description", "amount"] if not has_column_names else None
        ),  # If no column names, provide your own
    )
    return transaction_list_dataframe


def pdf_to_transaction_list(
    filepath: str,
    pdf_password: str,
    client=client,
    model="gpt-4-1106-preview",
) -> pd.DataFrame:
    # Generate the list dynamically
    column_list = ["Column" + str(i) for i in range(20)]
    # print(column_list)
    dfs = tabula.read_pdf(
        input_path=filepath,
        output_format="dataframe",
        encoding="utf-8",
        password=pdf_password,
        pages="all",
        multiple_tables=True,
        lattice=True,
        guess=False,
        pandas_options={
            "names": column_list,
            "header": None,
        },
    )
    clean_df = pd.DataFrame(columns=column_list)
    for i in dfs:
        temp_df = (
            i.replace("\r", " ", regex=True)
            .replace(r"(\d),(\d)", r"\1\2", regex=True)
            .replace(",", " ", regex=True)
        )
        clean_df = pd.concat([clean_df, temp_df], axis=0, ignore_index=True)
    clean_df = clean_df.dropna(axis=1, how="all")
    # Convert DataFrame to comma-separated string
    csv_string = clean_df.to_csv(index=False)
    prompt_str = (
        """
    Using the data provided from a bank statement, transform the transaction details into a standardized list format.
    Each transaction entry should be structured as following 3 columns: 'date, description, amount'.
    Ensure if the description contains amounts too, then remove it.
    Here's the raw data extracted for processing:
    """
        + csv_string
        + """
    Transform this data into a clean, readable list of transactions, adhering to the specified format.
    Ensure to include the negative sign for deductions from the account.
    Ensure you provide the data nothing else in response"""
    )
    # let's verify the function above matches the OpenAI API response
    prompt_messgae = [
        {
            "role": "user",
            "content": prompt_str,
        },
    ]
    response = client.chat.completions.create(
        model=model, messages=prompt_messgae, temperature=0  # , max_tokens=1
    )
    model_response = response.choices[0].message.content
    return model_response_to_dataframe(model_response)


def audio_to_transaction_list(
    file_path, transcription_model="whisper-1", llm_model="gpt-4-1106-preview"
):
    file_name, file_extension = os.path.splitext(file_path)
    audio_file = None
    if file_extension != ".mp3":
        AudioSegment.from_file(file_path, format="m4a").export(
            f"{file_name}.mp3", format="mp3"
        )
        audio_file = open(f"{file_name}.mp3", "rb")
    else:
        audio_file = open(file_path, "rb")
    transcription = client.audio.transcriptions.create(
        model=transcription_model, file=audio_file
    )
    response = client.chat.completions.create(
        model=llm_model,
        temperature=0,
        messages=[
            {
                "role": "user",
                "content": """
                    Using the transcription provided from a voice recording, create a list of transaction details into a standardized list format.
                    Each transaction entry should be structured as following 3 columns: 'date, description, amount'.
                    Here's the raw transcription extracted for processing:
                """
                + transcription.text
                + """
                Transform this data into a clean, readable list of transactions, adhering to the specified format.
                Ensure to include the negative sign for deductions from the account.
                Ensure you provide the data nothing else in response""",
            },
        ],
    )
    model_response = response.choices[0].message.content
    return model_response_to_dataframe(model_response)


if __name__ == "__main__":
    file_name = "Interview 002.m4a"
    file_path = root_dir + r"/data/audio/" + file_name
    print(audio_to_transaction_list(file_path))

    file_name = "2020 - 02 - February.pdf"
    filepath = root_dir + f"/data/bankstatements/{file_name}"
    pdf_password = "11323650"
    print(pdf_to_transaction_list(filepath, pdf_password))
