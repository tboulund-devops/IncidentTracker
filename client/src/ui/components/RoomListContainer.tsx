import React, { useEffect, useState } from "react";
import { useAtom } from "jotai";
import { chatApi } from "@core/controllers/chatApi";
import {allchatRoomsAtom, chatRoomsAtom} from "@core/atoms/chatAtoms";

export default function RoomListContainer({onSelectRoom}: { onSelectRoom: (room: any) => void}) {
    const [chatRooms, setChatRooms] = useAtom(chatRoomsAtom);
	const [allChatRooms, setAllChatRooms] = useAtom(allchatRoomsAtom);

	const isMember = (roomId: string) =>
		chatRooms.some(r => r.id === roomId);

	useEffect(() => {
		const fetchMyRooms = async () => {
			await chatApi.getMyRooms().then((userRooms) => {
                console.log("Fetched user rooms:", userRooms);
                setChatRooms(userRooms);
            }).catch((err: any) => {
                console.error('Failed to fetch rooms:', err);
                alert(err.message || 'Failed to fetch rooms');
            });
            //
		};
		fetchMyRooms();
	}, []);

	useEffect(() => {
		const fetchAllRooms = async () => {
			await chatApi.getAllRooms().then((allRooms) => {
				console.log("Fetched all rooms:", allRooms);
				setAllChatRooms(allRooms);
			}).catch((err: any) => {
				console.error('Failed to fetch rooms:', err);
				alert(err.message || 'Failed to fetch rooms');
			});
			//
		};
		fetchAllRooms();
	}, []);

	const handleJoin = async (roomId: string) => {
		await chatApi.joinRoom(roomId);
		const updated = await chatApi.getMyRooms();
		setChatRooms(updated);
	};

	const handleLeave = async (roomId: string) => {
		await chatApi.leaveRoom(roomId);
		const updated = await chatApi.getMyRooms();
		setChatRooms(updated);
	};


	return (
		<div>
			<h2>All Rooms</h2>
			<ul>
				{allChatRooms.map((room: any) => (
					<li key={room.id}>
						{room.name}
						{!isMember(room.id) && (
							<button onClick={() =>
								handleJoin(room.id)}>Join</button>
						)}
					</li>
				))}
			</ul>

			<h2>Your Rooms</h2>
			<ul>
				{chatRooms.map(room => (
					<li key={room.id}>
						{room.name}
						<button onClick={() => handleLeave(room.id)}>Leave</button>
						<button onClick={() => onSelectRoom(room)}>Open</button>
					</li>
				))}
			</ul>
		</div>
	);
}
