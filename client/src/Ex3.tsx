import {useState, useEffect} from 'react'
const es = new EventSource("http://localhost:5208/chat/stream");

export default function Ex3() {
  const [messages, setMessages] = useState<any[]>([])

    useEffect(() => {
      es.onmessage = (event) => {
        setMessages((prev) => [...prev, event.data])

      };

    }, [es.readyState]);
    
  return (
    <>

      {
        JSON.stringify(messages)
      }
      <button onClick={() => {
        fetch('http://localhost:5208/chat/send', {
          method: "POST",
          body: JSON.stringify({content: "hi"}),
          headers: {
            "Content-Type": "application/json"
          }
        })
      }}>send</button>
    </>
  )
}

