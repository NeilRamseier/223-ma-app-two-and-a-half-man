import { Component } from '@angular/core';
import { LedgerService } from '../../../services/ledger.service';
import { LeadingComment } from '@angular/compiler';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Ledger } from '../../../models/ledger.interface';

@Component({
  selector: 'app-create',
  imports: [FormsModule],
  templateUrl: './create.component.html',
  styleUrl: './create.component.css'
})
export class CreateComponent {

  fromId: number | null = null;
  name: string | null = null;
  balance: number | null = null;

  constructor(private lederService: LedgerService,
    private router: Router
  ) {}

  message = "";

  create(): void {

    if (this.fromId != null && this.name != null && this.balance!= null){
      this.lederService.create(this.fromId, this.name, this.balance).subscribe({
        next: () => {
            this.message = "Create successful!"
        },
        error: (error) => {
          this.message = "Create was not successful, please check your input!"
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
