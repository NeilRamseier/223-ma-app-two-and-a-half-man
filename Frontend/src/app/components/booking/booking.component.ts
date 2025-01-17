import { Component, OnInit } from '@angular/core';
import { BookingService } from '../../services/booking.service';
import { Router } from '@angular/router';
import { LedgerComponent } from '../ledger/ledger.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-booking',
  imports: [FormsModule],
  templateUrl: './booking.component.html',
  styleUrl: './booking.component.css'
})

export class BookingComponent {
  fromId: number | null = null;
  toId: number | null = null;
  amount: number | null = null;

      constructor(private bookingService: BookingService,
        private router: Router
      ) {}

    message = "";

    book(): void {

      if (this.fromId != null && this.toId != null && this.amount!= null){
        this.bookingService.book(this.fromId, this.toId, this.amount).subscribe({
          next: () => {
              this.message = "Booking successful!"
          },
          error: (error) => {
            this.message = "Booking was not successful, please check your input!"
          }
        });
      } else {
        this.message = "Please fill in all fields with valid data."
      }
    }

    navigateToLedgers(): void {
      this.router.navigate(['/ledgers']);
    }
}
