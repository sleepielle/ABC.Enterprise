import {cars} from '../db.js';
import amqp from "amqplib";
let gCars = {};



async function sendToRabbitMQ( message) {

    try {
      // Connect to RabbitMQ server
      const connection = await amqp.connect("amqp://localhost:5672");
  
      // Create a channel
      const channel = await connection.createChannel();
  
      // Define exchange name and type
      const exchangeName = "cars";
      const exchangeType = "direct";
  
      // Create exchange if it doesn't exist
      await channel.assertExchange(exchangeName, exchangeType, {
        durable: false,
      });
  
      const queueName = "carsQueue";
      await channel.assertQueue(queueName, {
        durable: true,
      });
  
  
  
      // Define message to send
     
  
      // Define routing key
      const routingKey = "myRoutingKey";
  
      // Publish message to exchange with confirm option
      const isSent = await channel.publish(
        exchangeName,
        routingKey,
        Buffer.from(message),
        { confirm: true }
      );
  
  
      channel.sendToQueue(queueName, Buffer.from(message), {
        persistent: true,
     });
  
      if (isSent) {
          console.log(`Sent message to ${queueName}: ${message}`);
      } else {
        console.log("Message could not be sent to RabbitMQ");
      }
  
      await channel.close();
      await connection.close();
    } catch (error) {
      console.error(error);
    }
      
  }

export const getCarsById = async (req, res) =>{
    const id = req.params.car_id;
    const [gCars] = await cars.query("SELECT id,make,model,year,branch_id FROM cars WHERE id = ?", [id]);
    
  
  
    res.json(gCars[0]);

    console.table(gCars);
  
    
}




  export {gCars};