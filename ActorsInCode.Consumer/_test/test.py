import json
from kafka import KafkaProducer
producer = KafkaProducer(bootstrap_servers=['127.0.0.1:9092'], api_version=(0, 10), value_serializer=lambda m: json.dumps(m).encode('ascii'))
# produce asynchronously
i = 0
while i < 1:
    producer.send("weather_forecast_topic", value = {
                    "date": "2024-12-28",
                     "temperatureC": 29,
                     "temperatureF": 84,
                     "summary": "Balmy",
                     "extraData": None
    
    })
    
i = i + 1
print("producer") 
producer.flush()