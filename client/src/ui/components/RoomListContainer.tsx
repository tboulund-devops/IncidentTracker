import React, { useEffect, useState } from "react";
import { useAtom } from "jotai";
import { chatApi } from "@core/controllers/chatApi";
import { chatRoomsAtom } from "@core/atoms/chatAtoms";

export default function RoomListContainer() {
    const [chatRooms, setChatRooms] = useAtom(chatRoomsAtom);

	useEffect(() => {
		const fetchRooms = async () => {
			await chatApi.getMyRooms().then((userRooms) => {
                console.log("Fetched user rooms:", userRooms);
                setChatRooms(userRooms);
            }).catch((err: any) => {
                console.error('Failed to fetch rooms:', err);
                alert(err.message || 'Failed to fetch rooms');
            });
            //
		};
		fetchRooms();
	}, []);

	return (
		<div>
			<h2>Your Rooms</h2>
			<ul>
				{chatRooms.map((room: any) => (
					<li key={room.id}>{room.name}</li>
				))}
			</ul>
		</div>
	);
}
