import React, { useState, useEffect } from "react";
import { useSse } from "@utils/useSse";
import { authAtom } from "@core/atoms/authAtom";
import { useAtom } from "jotai";
import { api } from "@utils/api";
import { chatApi } from "@core/controllers/chatApi";
import RoomListContainer from "@ui/components/RoomListContainer";

export default function HomePage() {
  const [searchTerm, setSearchTerm] = useState("");

  const handleSearch = async () => {
    await chatApi.searchRooms(searchTerm)
      .then((rooms) => {
        console.log("Search results:", rooms);
        // Here you would typically update your state with the search results
      })
      .catch((err: any) => {
        console.error('Search failed:', err);
        alert(err.message || 'Search failed'); 
      });
  };

  return (
    <div style={{ padding: 32 }}>
      <RoomListContainer/>
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
      </div>
    </div>
  );
}