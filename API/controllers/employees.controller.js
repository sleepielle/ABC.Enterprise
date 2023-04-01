
import {employees}  from '../db.js';
import amqp from "amqplib";
var gEmployees={};


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
  
      const queueName = "employeesQueue";
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


  export const getEmployeesByUsername = async (req, res)=> {
   
    const [gEmployees] = await employees.query(`SELECT username,first_name, last_name, ID,branch_id FROM employees WHERE username = ?`, [req.params.username] );
    res.json(gEmployees[0]);

    console.table(gEmployees);
  
}
/*export const getEmployeesByUsername = async (req, res)=> {
   
    const [gEmployees] = await employees.query(`SELECT username,first_name, last_name, ID FROM employees WHERE username = ?`, [req.params.username] );
     res.json(gEmployees);


     var message=JSON.stringify(gEmployees[0], null, 4)
    console.table(gEmployees);
    await sendToRabbitMQ(message);
}*/




  export {gEmployees};
 


