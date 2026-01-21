# how to test the chat exercise

## 1) start one or more terminal(s) representing web clients that subscribe:

```bash
curl -N "http://localhost:5208/chat/stream" & sleep 20

```
## 2) Then start another terminal representing a web client that "broadcasts" to web clients:

```bash
curl -X POST http://localhost:5208/chat/send \
    -H "Content-Type: application/json" \
    -d '{"Content":"Hello from curl!","GroupId":"room1"}'

```