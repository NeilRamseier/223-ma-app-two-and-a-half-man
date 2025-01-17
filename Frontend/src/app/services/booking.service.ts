import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking } from '../models/booking.interface';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root',
})
export class BookingService {
    private apiUrl = 'http://localhost:5000/api/v1';

    constructor(
        private http: HttpClient,
        private authService: AuthService,
    ) {}
/*
    getBookings(): Observable<Booking[]> {
        const token = this.authService.getToken();
        if (token) {
            return this.http.get<Booking[]>(`${this.apiUrl}/Bookings`);
        }

        return new Observable<Booking[]>();
    }
*/

    book(sourceId: number, destinationId: number, amount: number) {
        const booking = {
            sourceId,
            destinationId,
            amount,
        };
            return this.http.post(`${this.apiUrl}/Bookings`, booking)
    }
}
