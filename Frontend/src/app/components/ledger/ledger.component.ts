import { Component, OnInit } from '@angular/core';
import { LedgerService } from '../../services/ledger.service';
import { Ledger } from '../../models/ledger.interface';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-ledger',
    templateUrl: './ledger.component.html',
    imports: [CommonModule, FormsModule],
    providers: [LedgerService, HttpClient],
})
export class LedgerComponent implements OnInit {
    ledgers: Ledger[] = [];
    fromLedgerId: number | null = null;
    toLedgerId: number | null = null;
    amount: number | null = null;
    transferMessage = '';
    isEnabled: boolean = false;

    constructor(private ledgerService: LedgerService,
        private router: Router, private authService: AuthService
    ) {}

    ngOnInit(): void {
        this.loadLedgers();
        this.checkAuthorization();
    }

    checkAuthorization(): void {
        let authorized = this.authService.isLoggedIn();
        if(authorized){
            this.isEnabled = true;
        } else {
            this.isEnabled = false;
        }
    }

    loadLedgers(): void {
        this.ledgerService.getLedgers().subscribe({
            next: (data: Ledger[]) => {
                this.ledgers = data;
            },
            error: (error) => {
                console.error('Error fetching ledgers', error);
            },
        });
    }
    deleteLedger(ledgerId: number): void {
        this.ledgerService.deleteLedger(ledgerId).subscribe({
            next: () => {
                this.loadLedgers();
            },
            error: (error) => {
                console.error('Deletion error', error);
            },
        });
    }

    makeTransfer(): void {
        if (this.fromLedgerId !== null && this.toLedgerId !== null && this.amount !== null && this.amount > 0) {
            this.ledgerService.transferFunds(this.fromLedgerId, this.toLedgerId, this.amount).subscribe({
                next: () => {
                    this.transferMessage = 'Transfer successful!';
                    this.loadLedgers();
                },
                error: (error) => {
                    this.transferMessage = `Transfer failed: ${error.error.message}`;
                    console.error('Transfer error', error);
                },
            });
        } else {
            this.transferMessage = 'Please fill in all fields with valid data.';
        }
    }

    navigateToBooking() : void {
        void this.router.navigate(['/booking']);
    }

    navigateToCreate(): void {
        void this.router.navigate(['/create' ])
    }

}
