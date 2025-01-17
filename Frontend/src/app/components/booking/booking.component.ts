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

export class BookingComponent implements OnInit {
  fromId: number | undefined;
  toId: number | undefined;

      constructor(private bookingService: BookingService,
        private router: Router
      ) {}
  ngOnInit(): void {
    this.toId = 1;
  }

    message = "";

    book(): void {

      const fromIdElement = document.getElementById("fromId") as HTMLInputElement;
      const amountElement = document.getElementById("amount") as HTMLInputElement;
    
      const fromId = parseInt(fromIdElement.value, 10);
      const amount = parseFloat(amountElement.value);

      if (fromId != null && this.toId != null && amount!= null){
        this.bookingService.book(fromId, this.toId, amount).subscribe({
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
