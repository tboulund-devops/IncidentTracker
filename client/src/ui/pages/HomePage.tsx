import React, { useState, useEffect } from "react";
import { useSse } from "@utils/useSse";
import { authAtom } from "@core/atoms/authAtom";
import { useAtom } from "jotai";
import { api } from "@utils/api";
import { chatApi } from "@core/controllers/chatApi";
import RoomListContainer from "@ui/components/RoomListContainer";
import ChatRoom from "@ui/components/ChatRoom.tsx";

export default function HomePage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [searchResults, setSearchResults] = useState<any[]>([]);
  const [activeRoom, setActiveRoom] = useState<any | null>(null);

  const handleSearch = async () => {
    try {
        const rooms =  await chatApi.searchRooms(searchTerm);
        setSearchResults(rooms);
    } catch (err: any) {
        alert(err.message || 'Failed to search rooms');
        console.error('Failed to search rooms:', err);
    }
  };

  return (
    <div style={{ padding: 32 }}>
      <RoomListContainer onSelectRoom={setActiveRoom}/>
        {activeRoom && <ChatRoom room ={activeRoom}/>}
      <h1>Welcome to the Incident Tracker</h1>
      <p>This is the home page. You can view and manage incidents here.</p>
      <div style={{ marginBottom: "1rem" }}>
        <input
          type="text"
          placeholder="Search rooms..."
          value={searchTerm}
          onChange={e => setSearchTerm(e.target.value)}
          style={{ padding: "0.5rem", marginRight: "0.5rem" }}
        />
        <button onClick={handleSearch} style={{ padding: "0.5rem 1rem" }}>
          Search
        </button>
          <h2>Search Results</h2>
          <ul>
            {searchResults.map((room: any) => (
              <li key={room.id}>{room.name} <button>Join</button></li>
            ))}
          </ul>
      </div>
    </div>
  );
}