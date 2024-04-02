import requests

# The URL to your endpoint
url = "http://127.0.0.1:5000/upload-pdf"

# The path to the PDF file you want to upload
file_path = r"./data/bankstatements/2020 - 02 - February.pdf"

# Password for the PDF (assuming the server accepts it this way)
pdf_password = "11323650"

# Open the file in binary mode
with open(file_path, "rb") as file:
    # Define the files in a dictionary. The key 'file' is the name of the form field.
    # Check your server's expected field name, it might be different.
    files = {"file": (file_path, file, "application/pdf")}

    data = {"password": pdf_password}

    # Send the POST request with the file and password
    response = requests.post(url, files=files, data=data)
# Print the server's response
print(response.text)

# Sample Output Data
"""
[{"amount":15000.0,"date":"05/02/2020","description":"MBTRF"},{"amount":-51712.25,"date":"11/02/2020","description":"UAEEXCHANGEBET"},{"amount":-40000.0,"date":"18/02/2020","description":"UAEEXCHANGEBET"},{"amount":-1201.36,"date":"22/02/2020","description":"CREDIT CARD PAYMNT"},{"amount":1547.54,"date":"22/02/2020","description":"MBTRF"},{"amount":-11250.0,"date":"03/02/2020","description":"I/W CLEARING CHEQUE"},{"amount":11250.0,"date":"03/02/2020","description":"CHQ RETRN:Insufficient Funds(INF)"},{"amount":-241.5,"date":"03/02/2020","description":"Cheque Return Charge"},{"amount":-5169.5,"date":"05/02/2020","description":"UAEEXCHANGEBETA"},{"amount":15000.0,"date":"05/02/2020","description":"MBTRF_B/O"},{"amount":-11775.0,"date":"13/02/2020","description":"MBTRF AED@Amlak Finance"},{"amount":-10.0,"date":"18/02/2020","description":"MBTRF AED@Jay ashok sing"},{"amount":1547.54,"date":"22/02/2020","description":"MBTRF_B/O"},{"amount":-5123.75,"date":"24/02/2020","description":"UAEEXCHANGEBETA"},{"amount":14500.0,"date":"24/02/2020","description":"SALARY"}]
"""
