import React, { useEffect, useState } from "react";
import { chatApi } from "@core/controllers/chatApi";

export default function ChatRoom({ room }: { room: any }) {
    const [messages, setMessages] = useState<any[]>([]);
    const [text, setText] = useState("");

    const formatDate = (iso: string) => {
        const d = new Date(iso);
        return d.toLocaleString("en-GB"); // dd/mm/yyyy, hh:mm:ss
    };

    const loadMessages = async () => {
        const r = await fetch(`/api/chat/rooms/${room.id}/messages`);
        const data = await r.json();
        setMessages(data.dto ?? data);
    };

    useEffect(() => {
        loadMessages();
    }, [room.id]);

    const send = async () => {
        if (!text.trim()) return;

        await chatApi.sendMessage(room.id, text);
        setText("");
        await loadMessages(); // refresh after send
    };

    return (
        <div>
            <h3>{room.name}</h3>

            <div style={{ border: "1px solid gray", height: 300, overflowY: "auto" }}>
                {messages.map(m => (
                    <div key={m.id}>
                        <b>{m.username}</b>
                        [{formatDate(m.createdAt)}]{" "}
                        [{m.sender.username}]
                        {""} {m.content}
                    </div>
                ))}
            </div>

            <input value={text}
                   onChange={e => setText(e.target.value)}
                    onKeyDown={e => e.key === "Enter" && send()}/>{" "}
            <button onClick={send}>Send</button>
        </div>
    );
}