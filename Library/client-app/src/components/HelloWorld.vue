<template>
    <v-data-table :headers="headers"
                  :items="books"
                  :search="search"
                  sort-by="name"
                  class="elevation-1">
        <template v-slot:top>
            <v-toolbar flat>
                <v-toolbar-title>Library</v-toolbar-title>
                <v-divider class="mx-4"
                           inset
                           vertical></v-divider>
                <v-spacer></v-spacer>
                <v-text-field v-model="search"
                              append-icon="search"
                              label="Search"
                              single-line
                              hide-details></v-text-field>
                <v-spacer></v-spacer>
                <v-dialog v-model="dialog"
                          max-width="500px">
                    <template v-slot:activator="{ on, attrs }">
                        <v-btn color="primary"
                               dark
                               class="mb-2"
                               v-bind="attrs"
                               v-on="on">
                            New Item
                        </v-btn>
                    </template>
                    <v-card>
                        <v-card-title>
                            <span class="headline">{{ formTitle }}</span>
                        </v-card-title>

                        <v-card-text>
                            <v-container>
                                <p v-if="errors.length">
                                    <b>Please correct the following error(s):</b>
                                    <ul>
                                        <li v-for="error in errors" :key="error">{{ error }}</li>
                                    </ul>
                                </p>
                                <v-row>
                                    <v-col cols="12"
                                           sm="6"
                                           md="4">
                                        <v-text-field v-model="editedItem.name"
                                                      label="Book name"></v-text-field>
                                    </v-col>
                                    <v-col cols="12"
                                           sm="6"
                                           md="4">
                                        <v-text-field v-model="editedItem.author"
                                                      label="Author"></v-text-field>
                                    </v-col>
                                    <v-col cols="12"
                                           sm="6"
                                           md="4">
                                        <v-text-field v-model="editedItem.year"
                                                      label="Year"></v-text-field>
                                    </v-col>
                                </v-row>
                            </v-container>
                        </v-card-text>

                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="blue darken-1"
                                   text
                                   @click="close">
                                Cancel
                            </v-btn>
                            <v-btn color="blue darken-1"
                                   text
                                   @click="save">
                                Save
                            </v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
                <v-dialog v-model="dialogDelete" max-width="500px">
                    <v-card>
                        <v-card-title class="headline">Are you sure you want to delete this item?</v-card-title>
                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="blue darken-1" text @click="closeDelete">Cancel</v-btn>
                            <v-btn color="blue darken-1" text @click="deleteItemConfirm">OK</v-btn>
                            <v-spacer></v-spacer>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-toolbar>
        </template>
        <template v-slot:item.actions="{ item }">
            <v-icon small
                    class="mr-2"
                    @click="editItem(item)">
                mdi-pencil
            </v-icon>
            <v-icon small
                    @click="deleteItem(item)">
                mdi-delete
            </v-icon>
        </template>
        <template v-slot:no-data>
            <v-btn color="primary"
                   @click="initialize">
                Reset
            </v-btn>
        </template>
        <template v-slot:no-results>
            <v-alert :value="true" color="error" icon="warning">
                Your search for "{{ search }}" found no results.
            </v-alert>
        </template>
    </v-data-table>
</template>

<script>
    export default {
        data: () => ({
            search: '',
            errors: [],
            dialog: false,
            dialogDelete: false,
            headers: [
                {
                    text: 'Name of the book',
                    align: 'start',
                    value: 'name'
                },
                { text: 'Author', value: 'author' },
                { text: 'Year', value: 'year' },
                { text: 'Actions', value: 'actions', sortable: false }
            ],
            books: [],
            editedIndex: -1,
            editedId: 0,
            editedItem: {
                name: '',
                author: '',
                year: 0
            },
            defaultItem: {
                name: '',
                author: '',
                year: 0
            }
        }),

        computed: {
            formTitle() {
                return this.editedIndex === -1 ? 'New Item' : 'Edit Item'
            }
        },

        watch: {
            dialog(val) {
                val || this.close()
            },
            dialogDelete(val) {
                val || this.closeDelete()
            },
        },

        created() {
            this.initialize()
        },


        methods: {
            initialize() {
                this.books = [
                    {
                        name: 'test',
                        author: 'test',
                        year: 1922
                    }
                ]
            },

            checkForm: function () {
                if (this.editedItem.name && this.editedItem.author && !isNaN(this.editedItem.year)) {
                    return true;
                }

                this.errors = [];

                if (!this.editedItem.name) {
                    this.errors.push('Name of the book required!');
                }
                if (!this.editedItem.author) {
                    this.errors.push('Author required!');
                }
                if (isNaN(this.editedItem.year)) {
                    this.errors.push('Year must be a number!');
                }
            },

            editItem(item) {
                this.editedIndex = this.books.indexOf(item)
                this.editedItem = Object.assign({}, item)
                this.dialog = true
            },

            deleteItem(item) {
                this.editedIndex = this.books.indexOf(item)
                this.editedId = this.books[this.editedIndex].id
                this.editedItem = Object.assign({}, item)
                this.dialogDelete = true
            },

            deleteItemConfirm() {
                this.books.splice(this.editedIndex, 1)
                this.closeDelete()

                this.axios.delete('api/values/' + this.editedId)
            },

            close() {
                this.dialog = false
                this.$nextTick(() => {
                    this.editedItem = Object.assign({}, this.defaultItem)
                    this.editedIndex = -1
                })
            },

            closeDelete() {
                this.dialogDelete = false
                this.$nextTick(() => {
                    this.editedItem = Object.assign({}, this.defaultItem)
                    this.editedIndex = -1
                })
            },

            save() {
                if (this.checkForm()) {
                    if (this.editedIndex > -1) {
                        Object.assign(this.books[this.editedIndex], this.editedItem)

                        this.axios.put('api/values/', {
                            id: this.books[this.editedIndex].id,
                            name: this.editedItem.name,
                            author: this.editedItem.author,
                            year: +this.editedItem.year
                        })
                    } else {
                        this.books.push(this.editedItem)

                        this.axios.post('api/values/', {
                            name: this.editedItem.name,
                            author: this.editedItem.author,
                            year: +this.editedItem.year
                        }, {
                            headers: { 'Content-Type': 'application/json' }
                        })
                    }
                    this.close()
                }
            }
        },
        mounted() {
            this.axios
                .get('api/values')
                .then(response => (this.books = response.data));
        }
    }
</script>