import snowflake.connector

private_key_path = '/path/to/rsa_key_pkcs8.pem'

# Load private key
with open(private_key_path, 'rb') as key_file:
    private_key = key_file.read()

# Connect to Snowflake
connection = snowflake.connector.connect(
    user='<your_snowflake_username>',
    account='<your_snowflake_account>',
    private_key=private_key,
    warehouse='<your_warehouse>',
    database='<your_database>',
    schema='<your_schema>'
)

# Test a query
cursor = connection.cursor()
cursor.execute("SELECT CURRENT_VERSION()")
version = cursor.fetchone()
print(f"Connected to Snowflake version: {version}")
