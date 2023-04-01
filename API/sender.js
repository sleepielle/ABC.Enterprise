/*import amqp from "amqplib";

async function sendToRabbitMQ() {
  try {
    // Connect to RabbitMQ server
    const connection = await amqp.connect("amqp://localhost:5672");

    // Create a channel
    const channel = await connection.createChannel();

    // Define exchange name and type
    const exchangeName = "MECHE";
    const exchangeType = "direct";

    // Create exchange if it doesn't exist
    await channel.assertExchange(exchangeName, exchangeType, {
      durable: false,
    });

    const queueName = "myQueue";
    await channel.assertQueue(queueName, {
      durable: true,
    });



    // Define message to send
    const message = "Hello, RabbitMQ from Meche!";

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
      console.log("Message sent to RabbitMQ");
    } else {
      console.log("Message could not be sent to RabbitMQ");
    }

    // Close channel and connection
    await channel.close();
    await connection.close();
  } catch (error) {
    console.error(error);
  }
}

sendToRabbitMQ();
*/